using System;
using CommentService.Services.Persistence;
using CommentService.Services.Persistence.Repository;

namespace CommentService.Persistence
{
    public sealed class CommentDataStorage : ICommentDataStorage
    {
        public CommentDataStorage(ICommentRepository commentRepository)
        {
            CommentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        public ICommentRepository CommentRepository { get; }
    }
}
