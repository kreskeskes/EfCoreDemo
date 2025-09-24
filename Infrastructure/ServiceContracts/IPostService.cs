namespace Infrastructure.ServiceContracts
{
    public interface IPostService
    {

        /// <summary>
        /// Returns a post with top-N comments ordered by CreatedAtUtc (server-side filtering).
        /// </summary>
        Task<Post?> GetWithTopCommentsAsync(int postId, int topN, bool newestFirst = true, CancellationToken ct = default);

        // === EF Core 7+ batch operations ===============================================
        /// <summary>
        /// Deletes old comments by first materializing them and then removing (for demo contrast).
        /// </summary>
        Task<int> DeleteOldComments_MaterializeAsync(int postId, int olderThanDays, CancellationToken ct = default);

        /// <summary>
        /// Deletes old comments using ExecuteDelete (single server-side command).
        /// </summary>
        Task<int> DeleteOldComments_NoMaterializeAsync(int postId, int olderThanDays, CancellationToken ct = default);

        /// <summary>
        /// Creates a new post.
        /// </summary>
        Task<Post> CreateAsync(Post post, CancellationToken ct = default);

        /// <summary>
        /// Deletes a post (behavior depends on related FK DeleteBehavior).
        /// </summary>
        Task<bool> DeleteAsync(int postId, CancellationToken ct = default);

        /// <summary>
        /// Loads a detailed post graph using AsSplitQuery to avoid cartesian explosion.
        /// </summary>
        Task<List<Post>> GetPostsDetailed_AsSplit(CancellationToken ct = default);

        /// <summary>
        /// Loads a detailed post graph as a single query (demonstrates cartesian explosion risk).
        /// </summary>
        Task<List<Post>> GetPostsDetailed_AsSingle(CancellationToken ct = default);

    }
}