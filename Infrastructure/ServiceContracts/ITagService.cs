namespace Infrastructure.ServiceContracts
{
    public interface ITagService
    {
        /// <summary>
        /// Returns all tags.
        /// </summary>
        Task<List<Tag>> GetAll();

        /// <summary>
        /// Returns a tag by id.
        /// </summary>
        Task<Tag> GetById(int id);

        /// <summary>
        /// Updates an existing tag by id.
        /// </summary>
        Task<Tag> Update(Tag tagUpdateData, int id);

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        Task<Tag> CreateAsync(Tag tag, CancellationToken ct = default);

        /// <summary>
        /// Deletes a tag by id.
        /// </summary>
        Task<bool> DeleteAsync(int tagId, CancellationToken ct = default);
    }
}