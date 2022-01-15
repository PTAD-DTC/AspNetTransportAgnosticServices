using CommentService.BusinessLogic;
using CommentService.Core;
using CommentService.DtoBlMapper;
using CommentService.Interface;
using CommentService.Persistence;
using CommentService.Persistence.Repository;
using ForecastService.CommentService;

namespace ForecastServiceBenchmarks
{
    internal sealed class CommentServiceFactory : ICommentServiceFactory
    {
        private readonly Lazy<ICommentService> commentService = new(
            () => new CommentServiceCore(
                new CommentServiceLogic(
                    new CommentDataStorage(
                        new CommentMemoryRepository())),
                new DtoBlMapper()),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public ICommentService GetCommentService() => commentService.Value;
    }
}
