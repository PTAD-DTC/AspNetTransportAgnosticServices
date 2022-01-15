using CommentService.Services.Persistence.Repository;

namespace CommentService.Services.Persistence
{
    public interface ICommentDataStorage
    {
        ICommentRepository CommentRepository { get; }
    }
}
