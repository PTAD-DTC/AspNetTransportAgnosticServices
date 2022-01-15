using System;
using JetBrains.Annotations;
using Service.Interface.Base;

namespace CommentService.Interface.Models.DTO
{
    /// <summary>
    /// User Comment
    /// </summary>
    [PublicAPI]
    public sealed class CommentDto : ModelBase
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        private CommentDto()
        {

        }

        /// <summary>
        /// ctor
        /// </summary>
        public CommentDto(Guid id, Guid commentSubjectId, CommentDataDto commentData)
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
        public CommentDataDto? CommentData { get; set; }
    }
}
