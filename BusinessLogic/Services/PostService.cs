using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    internal class PostService(ApplicationDbContext dbContext) : IPostService
    {
        // === Server-side delete: materialize vs no-materialize ====================
        public Task<int> DeleteOldComments_NoMaterializeAsync(int postId, int olderThanDays, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddDays(-olderThanDays);

            return dbContext.Comments
                .Where(c => c.PostId == postId && c.CreatedAtUtc < cutoff)
                .ExecuteDeleteAsync(ct);
        }

        public async Task<int> DeleteOldComments_MaterializeAsync(int postId, int olderThanDays, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddDays(-olderThanDays);

            var items = await dbContext.Comments
                .Where(c => c.PostId == postId && c.CreatedAtUtc < cutoff)
                .ToListAsync(ct);

            dbContext.Comments.RemoveRange(items);
            return await dbContext.SaveChangesAsync(ct);
        }

        // === Filtered/ordered Include ============================================
        public Task<Post?> GetWithTopCommentsAsync(int postId, int topN, bool newestFirst = true, CancellationToken ct = default)
        {
            IQueryable<Post> q = dbContext.Posts.Where(p => p.Id == postId);
            if (newestFirst)
            {
                q = q.Include(p => p.Comments
                        .OrderByDescending(c => c.CreatedAtUtc)
                        .Take(topN));
            }
            else
            {
                q = q.Include(p => p.Comments
                        .OrderBy(c => c.CreatedAtUtc)
                        .Take(topN));
            }

            return q.FirstOrDefaultAsync(ct);
        }

        // === Basic create, delete =================================================================
        public async Task<Post> CreateAsync(Post post, CancellationToken ct = default)
        {
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync(ct);
            return post;
        }

        public async Task<bool> DeleteAsync(int postId, CancellationToken ct = default)
        {
            var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId, ct);
            if (post is null) return false;

            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync(ct);
            return true;
        }

    }
}