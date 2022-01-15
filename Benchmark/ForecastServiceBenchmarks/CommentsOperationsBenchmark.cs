using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using Service.Interface.Base;

namespace ForecastServiceBenchmarks
{
    //[SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class CommentsOperationsBenchmark : ForecastBenchmarkBase
    {
        protected override async Task DoTest(IForecastService service, CancellationToken cancellationToken)
        {
            var forecastsCallResult = await service.GetTodayForecasts(cancellationToken);
            if (forecastsCallResult.ResultCode != ResultCode.Ok)
                return;

            foreach (var forecast in forecastsCallResult.Result!)
            {
                //for (int i = 0; i < 1; i++)
                //{
                //    await service.AddComment(forecast.Id, new ForecastCommentDataDto($"Test comment No {i}"), cancellationToken);
                //}
                var addCommentCallResult = await service.AddComment(forecast.Id, new ForecastCommentDataDto("Comment to be updated"), cancellationToken);
                if (addCommentCallResult.ResultCode == ResultCode.Ok)
                {
                    await service.UpdateComment(forecast.Id, addCommentCallResult.Result!.Id, new ForecastCommentDataDto("Updated"), cancellationToken);
                }

                //await service.GetForecastComments(forecast.Id, cancellationToken);
            }
        }
    }
}
