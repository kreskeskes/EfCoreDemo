using Infrastructure.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastuctureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                // .UseLazyLoadingProxies();              // enable proxies

            });

            services.AddScoped<DbSeeder>();
            return services;
        }

        public static async Task UseInfrastructureAsync(this IApplicationBuilder app, CancellationToken cancellationToken = default)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync(cancellationToken);
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync(cancellationToken);
            }
        }
    }
}