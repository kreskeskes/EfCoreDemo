namespace Domain.Models
{
    public class Post : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int BlogId { get; set; }
        public virtual Blog? Blog { get; set; }

        public int AuthorId { get; set; }
        public virtual User? Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<PostTag> PostTags { get; set; } = [];
    }
}