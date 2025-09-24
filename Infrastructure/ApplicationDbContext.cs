namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostTag> PostTags => Set<PostTag>();
        public DbSet<Tag> Tags => Set<Tag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            foreach (var e in ChangeTracker.Entries<BaseEntity>())
            {
                //if (e.State == EntityState.Added) e.Entity.CreatedAtUtc = now;
                if (e.State == EntityState.Modified) e.Entity.UpdatedAtUtc = now;
            }
            return base.SaveChangesAsync(ct);
        }
    }
}