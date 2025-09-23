namespace Infrastructure.ServiceContracts
{
    public interface IBlogService
    {
        /// <summary>
        /// Loads a detailed blog graph using AsSplitQuery to avoid cartesian explosion.
        /// </summary>
        Task<List<Blog>> GetBlogsDetailed_AsSplit(CancellationToken ct = default);

        /// <summary>
        /// Loads a detailed blog graph as a single query (demonstrates cartesian explosion risk).
        /// </summary>
        Task<List<Blog>> GetBlogsDetailed_AsSingle(CancellationToken ct = default);

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