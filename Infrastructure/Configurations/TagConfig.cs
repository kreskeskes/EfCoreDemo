namespace Infrastructure.Configurations
{
    internal class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();


            builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();
        }
    }
}