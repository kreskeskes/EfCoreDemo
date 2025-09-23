namespace Domain.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = null!;

        public virtual ICollection<PostTag> PostTags { get; set; } = [];
    }
}