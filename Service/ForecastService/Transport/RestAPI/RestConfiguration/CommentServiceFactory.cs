using System;
using CommentService.Interface;
using ForecastService.CommentService;
using Microsoft.Extensions.DependencyInjection;

namespace ForecastService.RestServerConfiguration
{
    internal sealed class CommentServiceFactory : ICommentServiceFactory
    {
        private IServiceProvider _serviceProvider;

        public CommentServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ICommentService GetCommentService()
        {
            return _serviceProvider.GetRequiredService<ICommentService>();
        }
    }
}
