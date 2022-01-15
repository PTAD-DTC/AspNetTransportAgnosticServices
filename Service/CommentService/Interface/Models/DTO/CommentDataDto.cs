using JetBrains.Annotations;
using Service.Interface.Base;

namespace CommentService.Interface.Models.DTO
{
    /// <summary>
    /// User comment data
    /// </summary>
    [PublicAPI]
    public sealed class CommentDataDto : ModelBase
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        private CommentDataDto()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public CommentDataDto(string comment)
        {
            Comment = comment;
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string? Comment { get; set; }
    }
}
