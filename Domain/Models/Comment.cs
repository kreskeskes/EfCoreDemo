namespace Domain.Models
{
    public class Comment : BaseEntity
    {
        public int PostId { get; set; }
        public virtual Post? Post { get; set; }

        public string? AuthorName { get; set; }
        public string Text { get; set; } = null!;
    }
}