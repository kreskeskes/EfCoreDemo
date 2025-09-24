namespace Infrastructure.ServiceContracts
{
    public interface IUserService
    {
        /// <summary>
        /// Updates user name in tracked state.
        /// </summary>
        Task<User?> UpdateNameTrackedAsync(int id, string newName, CancellationToken ct = default);

        /// <summary>
        /// Updates user name with AsNoTracking.
        /// </summary>
        Task<User?> UpdateNameNotTrackedAsync(int id, string newName, CancellationToken ct = default);

        /// <summary>
        /// Reloads the entity from database using Entry.ReloadAsync.
        /// </summary>
        Task<User> ReloadAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Transfers blogs and posts from one user to another in a single transaction.
        /// </summary>
        Task<bool> TransferBlogsAsync(int fromUserId, int toUserId, CancellationToken ct = default);


        /// <summary>
        /// Transfers blogs and posts from one user to another in a single transaction with savepoints.
        /// </summary>
        Task<bool> TransferBlogsWithSavepointsAsync(int fromUserId, int toUserId, CancellationToken ct = default);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        Task<User> CreateAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Deletes user.
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Gets user by id.
        /// </summary>
        Task<User?> GetById(int id, CancellationToken ct = default);

        /// <summary>
        /// Returns blogs by user id
        /// </summary>
        Task<List<User>> GetUserWithBlogsAndPosts(int Id, CancellationToken ct = default);
    }
}