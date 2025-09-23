namespace Domain.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Blog> Blogs { get; set; } = [];
        public virtual ICollection<Post> Posts { get; set; } = [];
    }
}