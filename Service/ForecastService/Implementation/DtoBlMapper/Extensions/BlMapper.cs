using ForecastService.Interface.Models.DTO;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.DtoBlMapper.Extensions
{
    internal static class BlMapper
    {
        public static CommentData? MapToBl_Safe(this ForecastCommentDataDto? commentData)
        {
            return (commentData?.Comment is null)
                ? null
                : new CommentData(commentData.Comment);
        }
    }
}
