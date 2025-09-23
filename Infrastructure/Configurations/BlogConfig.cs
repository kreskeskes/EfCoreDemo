namespace Infrastructure.Configurations
{
    internal class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blogs");

            builder.HasKey(b => b.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();


            builder.HasOne(b => b.Owner)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}