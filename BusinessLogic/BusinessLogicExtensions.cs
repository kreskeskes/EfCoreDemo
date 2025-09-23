using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic
{
    public static class BusinessLogicExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ITagService, TagService>();

            return services;
        }

    }
}

