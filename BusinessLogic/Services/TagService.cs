using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    internal class TagService(ApplicationDbContext dbContext) : ITagService
    {
        public async Task<Tag> CreateAsync(Tag tag, CancellationToken ct = default)
        {
            dbContext.Tags.Add(tag);
            await dbContext.SaveChangesAsync(ct);
            return tag;
        }

        public async Task<bool> DeleteAsync(int tagId, CancellationToken ct = default)
        {
            var existing = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId, ct);
            if (existing is null) return false;

            dbContext.Tags.Remove(existing);
            await dbContext.SaveChangesAsync(ct);
            return true;
        }

        public Task<List<Tag>> GetAll()
            => dbContext.Tags.AsNoTracking().ToListAsync();

        public async Task<Tag> GetById(int id)
        {
            var tag = await dbContext.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null) throw new KeyNotFoundException($"Tag {id} not found");
            return tag;
        }

        public async Task<Tag> Update(Tag tagUpdateData, int id)
        {
            var existing = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existing is null) throw new KeyNotFoundException($"Tag {id} not found");

            existing.Name = tagUpdateData.Name;

            await dbContext.SaveChangesAsync();
            return existing;
        }
    }
}