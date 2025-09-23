using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    internal class UserService(ApplicationDbContext dbContext) : IUserService
    {
        // === Tracking / NoTracking ===============================================
        public async Task<User?> UpdateNameNotTrackedAsync(int id, string newName, CancellationToken ct = default)
        {
            var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return null;

            user.Name = newName;
            var entry = dbContext.Entry(user);
            Console.WriteLine($"State: {entry.State}");

            foreach (var property in entry.Properties)
                Console.WriteLine($"{property.Metadata.Name}: current = {property.CurrentValue} original = {property.OriginalValue}");

            await dbContext.SaveChangesAsync();

            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> UpdateNameTrackedAsync(int id, string newName, CancellationToken ct = default)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return null;

            user.Name = newName;
            var entry = dbContext.Entry(user);
            Console.WriteLine($"State: {entry.State}");

            foreach (var property in entry.Properties)
                Console.WriteLine($"{property.Metadata.Name}: current = {property.CurrentValue} original = {property.OriginalValue}");

            await dbContext.SaveChangesAsync();
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> ReloadAsync(int id, CancellationToken ct = default)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null)
                return null;

            // Changing something
            user.Name = "Locally changed";

            // Reload will cancel the changes made to the entity and get the object state from Db
            await dbContext.Entry(user).ReloadAsync(ct);

            return user;
        }

        // === Transactions =========================================================
        public async Task<bool> TransferBlogsAsync(int fromUserId, int toUserId, CancellationToken ct = default)
        {
            if (fromUserId == toUserId)
                return true;

            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var users = await dbContext.Users
                    .Where(x => x.Id == fromUserId || x.Id == toUserId)
                    .Select(x => x.Id)
                    .ToListAsync(ct);

                if (!users.Contains(fromUserId) || !users.Contains(toUserId))
                    return false;

                var now = DateTime.UtcNow;

                var blogsUpdated = await dbContext.Blogs
                    .Where(x => x.OwnerId == fromUserId)
                    .TagWith("IUserService.TransferBlogs: reassign blog owners")
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.OwnerId, toUserId)
                        .SetProperty(x => x.UpdatedAtUtc, now),
                    ct);

                var postsUpdated = await dbContext.Posts
                    .Where(x => x.AuthorId == fromUserId)
                    .TagWith("IUserService.TransferBlogs: reassign post authors")
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.AuthorId, toUserId)
                        .SetProperty(x => x.UpdatedAtUtc, now),
                    ct);

                await transaction.CommitAsync(ct);

                return blogsUpdated > 0 || postsUpdated > 0;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                return false;
            }
        }

        // === Basic Create/Delete ==========================================================

        //Importance of IsUnique index
        public async Task<User> CreateAsync(User user, CancellationToken ct = default)
        {
            // It wouldn't save us if we have 2 simulatneous requests and no unique index in place for the property!
            if (await dbContext.Users.AnyAsync(u => u.Email == user.Email, ct))
                throw new InvalidOperationException($"User with email {user.Email} already exists");

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);

            return user;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null)
                return false;

            dbContext.Users.Remove(user);

            try
            {
                await dbContext.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Delete failed: {ex.InnerException?.Message}");
                return false;
            }
        }

        public async Task<User?> GetById(int id, CancellationToken ct = default) =>
         await dbContext.Users.Include(x => x.Blogs).FirstOrDefaultAsync(x => x.Id==id);


        public async Task<List<User>> GetUserWithBlogs(int id, CancellationToken ct = default)
        {
            return await dbContext.Users.Where(u => u.Id == id).Include(x => x.Blogs).ToListAsync(ct);
        }
    }
}