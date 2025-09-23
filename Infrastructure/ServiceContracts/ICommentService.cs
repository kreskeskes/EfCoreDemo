namespace Infrastructure.ServiceContracts
{
    public interface ICommentService
    {
        /// <summary>
        /// Creates a comment.
        /// </summary>
        Task<Comment> CreateAsync(Comment comment, CancellationToken ct = default);

        /// <summary>
        /// Deletes a comment by id.
        /// </summary>
        Task<bool> DeleteAsync(int commentId, CancellationToken ct = default);

        /// <summary>
        /// Gets a comment by id.
        /// </summary>
        Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Searches comments by author name using EF.Functions.Like (provider-agnostic).
        /// </summary>
        Task<IReadOnlyList<Comment>> SearchByAuthorAsync(string authorName, CancellationToken ct = default);

        Task<IReadOnlyList<Comment>> RawSearchByCommentAuthorName(string authorName, CancellationToken ct = default);
    }
}