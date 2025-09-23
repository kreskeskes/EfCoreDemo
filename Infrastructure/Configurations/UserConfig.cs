namespace Infrastructure.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.Name)
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(60)
                .IsRequired();

            //builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}