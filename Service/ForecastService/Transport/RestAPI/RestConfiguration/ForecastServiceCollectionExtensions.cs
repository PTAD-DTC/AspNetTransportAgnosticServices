using System;
using CommentService.Interface;
using ForecastService.BusinessLogic.Implementation;
using ForecastService.CommentService;
using ForecastService.Core;
using ForecastService.Interface;
using ForecastService.Persistence;
using ForecastService.Persistence.Repository;
using ForecastService.Services.BusinessLogic;
using ForecastService.Services.CommentService;
using ForecastService.Services.Persistence;
using ForecastService.Services.Persistence.Repository;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ForecastService.RestServerConfiguration
{
    [PublicAPI]
    public static class ForecastServiceCollectionExtensions
    {
        public static IServiceCollection UseForecastService(this IServiceCollection services)
        {
            services.AddScoped<IForecastRepository, ForecastMemoryRepository>();
            services.AddScoped<IForecastDataStorage, ForecastDataStorage>();
            services.AddScoped<IForecastServiceLogic, ForecastServiceLogic>();
            services.AddScoped<IDtoBlMapper, DtoBlMapper.DtoBlMapper>();
            services.AddScoped<IForecastService, ForecastServiceCore>();
            services.AddScoped<IForecastCommentService, ForecastCommentService>();
            services.AddScoped<ICommentServiceFactory, CommentServiceFactory>();

            return services;
        }

        public static IServiceCollection UseCommentServiceNativeClient(this IServiceCollection services, Uri serviceBaseUri)
        {
            services.AddScoped<ICommentService>(_ => new CommentClient.Native.HttpClient.CommentClient(serviceBaseUri));
            return services;
        }
    }
}
