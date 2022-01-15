using CommentService.BusinessLogic;
using CommentService.Core;
using CommentService.Interface;
using CommentService.Persistence;
using CommentService.Persistence.Repository;
using CommentService.Services.BusinessLogic;
using CommentService.Services.Persistence;
using CommentService.Services.Persistence.Repository;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace CommentService.RestServerConfiguration
{
    [PublicAPI]
    public static class CommentServiceCollectionExtensions
    {
        public static IServiceCollection UseCommentService(this IServiceCollection services)
        {
            services.AddScoped<ICommentRepository, CommentMemoryRepository>();
            services.AddScoped<ICommentDataStorage, CommentDataStorage>();
            services.AddScoped<ICommentServiceLogic, CommentServiceLogic>();
            services.AddScoped<IDtoBlMapper, DtoBlMapper.DtoBlMapper>();
            services.AddScoped<ICommentService, CommentServiceCore>();

            return services;
        }
    }
}
