namespace Domain.Models
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? OwnerId { get; set; }
        public virtual User? Owner { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = [];
    }
}