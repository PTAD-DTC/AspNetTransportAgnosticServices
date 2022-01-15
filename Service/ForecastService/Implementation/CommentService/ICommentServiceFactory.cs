using CommentService.Interface;

namespace ForecastService.CommentService
{
    public interface ICommentServiceFactory
    {
        ICommentService GetCommentService();
    }
}
