using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    internal class CommentService(ApplicationDbContext dbContext) : ICommentService
    {
        public async Task<Comment> CreateAsync(Comment comment, CancellationToken ct = default)
        {
            if (comment is null) throw new ArgumentNullException(nameof(comment));
            await dbContext.Comments.AddAsync(comment, ct);
            await dbContext.SaveChangesAsync(ct);
            return comment;
        }

        public async Task<bool> DeleteAsync(int commentId, CancellationToken ct = default)
        {
            var entity = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId, ct);
            if (entity is null) return false;

            dbContext.Comments.Remove(entity);
            await dbContext.SaveChangesAsync(ct);
            return true;
        }

        public Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default) =>
       dbContext.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<IReadOnlyList<Comment>> RawSearchByCommentAuthorName(string authorName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(authorName))
                throw new ArgumentException("Domain must not be empty", nameof(authorName));

            var comments = await dbContext.Comments
                .FromSqlInterpolated($"SELECT * FROM \"Comments\" WHERE \"AuthorName\" ILIKE {'%' + authorName + '%'} ")
                .AsNoTracking()
                .ToListAsync(ct);

            return comments;
        }

        public async Task<IReadOnlyList<Comment>> SearchByAuthorAsync(string authorName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(authorName))
                throw new ArgumentException("authorName must not be empty.", nameof(authorName));

            var pattern = $"%{authorName}%";
            return await dbContext.Comments
                .AsNoTracking()
                .Where(c => c.AuthorName != null && EF.Functions.ILike(c.AuthorName, pattern))
                .ToListAsync(ct);
        }
    }
}