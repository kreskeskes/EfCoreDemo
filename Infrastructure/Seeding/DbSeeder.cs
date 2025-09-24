using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Seeding
{
    public sealed class DbSeeder(ApplicationDbContext db, ILogger<DbSeeder> log)
    {
        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await db.Users.AsNoTracking().AnyAsync(cancellationToken)) return;

            var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SeedData");
            if (Directory.Exists(dataDirectory))
            {
                var users = await ReadAsync<List<User>>(Path.Combine(dataDirectory, "users.json"), cancellationToken) ?? [];
                var blogs = await ReadAsync<List<Blog>>(Path.Combine(dataDirectory, "blogs.json"), cancellationToken) ?? [];
                var posts = await ReadAsync<List<Post>>(Path.Combine(dataDirectory, "posts.json"), cancellationToken) ?? [];
                var comments = await ReadAsync<List<Comment>>(Path.Combine(dataDirectory, "comments.json"), cancellationToken) ?? [];
                var tags = await ReadAsync<List<Tag>>(Path.Combine(dataDirectory, "tags.json"), cancellationToken) ?? [];
                var postTags = await ReadAsync<List<PostTag>>(Path.Combine(dataDirectory, "postTags.json"), cancellationToken) ?? [];

                await db.Users.AddRangeAsync(users, cancellationToken);
                await db.Blogs.AddRangeAsync(blogs, cancellationToken);
                await db.Posts.AddRangeAsync(posts, cancellationToken);
                await db.Comments.AddRangeAsync(comments, cancellationToken);
                await db.Tags.AddRangeAsync(tags, cancellationToken);
                await db.PostTags.AddRangeAsync(postTags, cancellationToken);
            }
            else
            {
                // Fallback
                var alice = new User { Id = 1, Name = "Alice", Email = "alice@example.com" };
                var blog = new Blog { Id = 1, Title = "Alice's Tech Blog", OwnerId = 1 };
                var post = new Post { Id = 1, Title = "EF Core Basics", Content = "Intro...", BlogId = 1, AuthorId = 1 };
                var tag = new Tag { Id = 1, Name = "EFCore" };
                var link = new PostTag { PostId = 1, TagId = 1 };

                db.AddRange(alice, blog, post, tag, link);
            }

            await db.SaveChangesAsync(cancellationToken);

            await db.Database.ExecuteSqlRawAsync("""
                SELECT setval(pg_get_serial_sequence('"Users"',   'Id'), COALESCE((SELECT MAX("Id") FROM "Users"),   0));
                SELECT setval(pg_get_serial_sequence('"Blogs"',   'Id'), COALESCE((SELECT MAX("Id") FROM "Blogs"),   0));
                SELECT setval(pg_get_serial_sequence('"Posts"',   'Id'), COALESCE((SELECT MAX("Id") FROM "Posts"),   0));
                SELECT setval(pg_get_serial_sequence('"Comments"','Id'), COALESCE((SELECT MAX("Id") FROM "Comments"),0));
                SELECT setval(pg_get_serial_sequence('"Tags"',    'Id'), COALESCE((SELECT MAX("Id") FROM "Tags"),    0));
                """, cancellationToken);
            log.LogInformation("Database seeded");
        }

        private static async Task<T?> ReadAsync<T>(string path, CancellationToken cancellationToken)
        {
            if (!File.Exists(path)) return default;
            await using var s = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(s, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }, cancellationToken);
        }
    }
}