namespace Infrastructure.Configurations
{
    public class PostConfig : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();


            builder.Property(p => p.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Content)
                .IsRequired();

            builder.HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.Title);
        }
    }
}