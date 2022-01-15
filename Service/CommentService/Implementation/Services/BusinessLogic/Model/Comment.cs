using System;

namespace CommentService.Services.BusinessLogic.Model
{
    public sealed class Comment
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Comment(Guid id, Guid commentSubjectId, CommentData commentData)
        {
            Id = id;
            CommentSubjectId = commentSubjectId;
            CommentData = commentData ?? throw new ArgumentNullException(nameof(commentData));
        }

        /// <summary>
        /// Comment id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Subject id
        /// </summary>
        public Guid CommentSubjectId { get; set; }

        /// <summary>
        /// Comment data
        /// </summary>
        public CommentData? CommentData { get; set; }
    }
}
