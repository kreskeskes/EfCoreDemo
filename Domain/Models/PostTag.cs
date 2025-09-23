namespace Domain.Models
{
    public class PostTag
    {
        public int PostId { get; set; }
        public virtual Post Post { get; set; } = null!;

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;

        public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
    }
}