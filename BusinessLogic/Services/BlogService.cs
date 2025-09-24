using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BusinessLogic.Services
{
    public class BlogService(ApplicationDbContext dbContext, ILogger<BlogService> logger) : IBlogService
    {
        // === Explicit loading =====================================================
        public async Task<Blog?> LoadNavigationsExplicitlyAsync(int blogId, bool loadPosts = true, bool loadOwner = false, CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew();

            var blog = await dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == blogId, ct);

            if (blog is null)
                return null;

            if (loadPosts)
                await dbContext.Entry(blog).Collection(b => b.Posts).Query().TagWith("BlogFull:Explicit:Posts").LoadAsync(ct);

            if (loadOwner)
                await dbContext.Entry(blog).Reference(b => b.Owner).Query().LoadAsync(ct);

            sw.Stop();
            logger.LogInformation("Explicit took {Elapsed} ms", sw.ElapsedMilliseconds);
            return blog;
        }

        // === Basic create, delete =================================================================
        public async Task<Blog> CreateAsync(Blog blog, CancellationToken ct = default)
        {
            dbContext.Blogs.Add(blog);

            await dbContext.SaveChangesAsync(ct);

            return blog;
        }

        public async Task<bool> DeleteAsync(int blogId, CancellationToken ct = default)
        {
            var blog = await dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == blogId, ct);

            if (blog is null)
                return false;

            dbContext.Blogs.Remove(blog);

            await dbContext.SaveChangesAsync(ct);

            return true;
        }

    }
}