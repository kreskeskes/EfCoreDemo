namespace Infrastructure.ServiceContracts
{
    public interface IBlogService
    {
        /// <summary>
        /// Explicitly loads selected navigations (Posts/Owner) for a tracked blog.
        /// </summary>
        Task<Blog?> LoadNavigationsExplicitlyAsync(int blogId, bool loadPosts = true, bool loadOwner = false, CancellationToken ct = default);

        // === CRUD =================================================================

        /// <summary>
        /// Creates a new blog.
        /// </summary>
        Task<Blog> CreateAsync(Blog blog, CancellationToken ct = default);

        /// <summary>
        /// Permanently deletes a blog (subject to FK DeleteBehavior).
        /// </summary>
        Task<bool> DeleteAsync(int blogId, CancellationToken ct = default);
    }
}