using ForecastService.Interface.Models.DTO;
using Swashbuckle.AspNetCore.Filters;

namespace ForecastService.RestExamples.V2_0
{
    public sealed class ForecastCommentDataDtoExample : IExamplesProvider<ForecastCommentDataDto>
    {
        public ForecastCommentDataDto GetExamples()
        {
            return new ForecastCommentDataDto("forecast comment v2.0");
        }
    }
}
