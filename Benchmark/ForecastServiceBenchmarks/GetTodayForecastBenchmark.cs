using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ForecastService.Interface;

namespace ForecastServiceBenchmarks
{
    //[SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class GetTodayForecastBenchmark : ForecastBenchmarkBase
    {
        protected override Task DoTest(IForecastService service, CancellationToken cancellationToken) =>
            service.GetTodayForecasts(cancellationToken);
    }
}
