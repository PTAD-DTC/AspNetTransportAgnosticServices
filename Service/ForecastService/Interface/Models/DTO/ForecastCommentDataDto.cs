using JetBrains.Annotations;
using Service.Interface.Base;

namespace ForecastService.Interface.Models.DTO
{
    /// <summary>
    /// User comment data
    /// </summary>
    [PublicAPI]
    public sealed class ForecastCommentDataDto : ModelBase
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        private ForecastCommentDataDto()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public ForecastCommentDataDto(string comment)
        {
            Comment = comment;
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string? Comment { get; set; }
    }
}
