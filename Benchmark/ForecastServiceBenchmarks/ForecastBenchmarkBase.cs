using BenchmarkDotNet.Attributes;
using ForecastClient.Native.HttpClient;
using ForecastService.BusinessLogic.Implementation;
using ForecastService.CommentService;
using ForecastService.Core;
using ForecastService.DtoBlMapper;
using ForecastService.Interface;
using ForecastService.Persistence;
using ForecastService.Persistence.Repository;

namespace ForecastServiceBenchmarks
{
    public abstract class ForecastBenchmarkBase
    {
        private static readonly Uri DefaultLocalServiceUri = new("http://localhost:6002/");
        private static readonly Uri DefaultgRpcServiceUri = new("https://localhost:7060");
        //private static readonly Uri DefaultLocalServiceUri = new ("https://localhost:6001/");

        private static IForecastService CreateForecastServiceCoreV2S1() =>
            new ForecastServiceCore(
                new ForecastServiceLogic(
                    new ForecastDataStorage(
                        new ForecastMemoryRepository()),
                    new ForecastCommentService(
                        new CommentServiceFactory())),
                new DtoBlMapper());

        private static ForecastApiClient CreateHttpServiceClient(
            Uri serviceUri) => new(serviceUri);

        private static ForecastClient.Native.RestClient.ForecastApiClient CreateRestServiceClient(
            Uri serviceUri) => new(serviceUri);

        private static ForecastClient.Native.gRPC.ForecastGrpcClient CreategRpcServiceClient(
            Uri serviceUri) => new(serviceUri);

        private static void ServiceRestartRequest(Uri serviceUri)
        {
            Console.WriteLine("");
            Console.WriteLine($"Preparing next benchmark: {serviceUri}");
            Console.WriteLine($"Restart service at {serviceUri} ....");
            for (int i = 25; i > 0; i--)
            {
                Console.WriteLine($"Test will start in {i}");
                Thread.Sleep(1000);
            }
            Console.WriteLine("");
        }

        private IForecastService? forecastServiceCoreV2S1;

        [GlobalSetup(Target = nameof(ServiceCore))]
        public void PrepareForecastServiceCoreV2S1()
        {
            forecastServiceCoreV2S1 = CreateForecastServiceCoreV2S1();
        }

        [GlobalSetup(Target = nameof(TransportRestHttp))]
        public void PrepareForecastServiceHttpClient()
        {
            var uri = DefaultLocalServiceUri;
            ServiceRestartRequest(uri);
            forecastServiceCoreV2S1 = CreateHttpServiceClient(uri);
        }

        [GlobalSetup(Target = nameof(TransportRestClient))]
        public void PrepareForecastServiceRestClient()
        {
            var uri = DefaultLocalServiceUri;
            ServiceRestartRequest(uri);
            forecastServiceCoreV2S1 = CreateRestServiceClient(uri);
        }

        [GlobalSetup(Target = nameof(TransportgRpcClient))]
        public void PrepareForecastServicegRpcClient()
        {
            var uri = DefaultgRpcServiceUri;
            ServiceRestartRequest(uri);
            forecastServiceCoreV2S1 = CreategRpcServiceClient(uri);
        }

        [Benchmark(Baseline = true)]
        public Task ServiceCore() =>
            DoTest(forecastServiceCoreV2S1!, CancellationToken.None);

        [Benchmark()]
        public Task TransportRestHttp() =>
            DoTest(forecastServiceCoreV2S1!, CancellationToken.None);

        [Benchmark]
        public Task TransportRestClient() =>
            DoTest(forecastServiceCoreV2S1!, CancellationToken.None);

        [Benchmark]
        public Task TransportgRpcClient() =>
            DoTest(forecastServiceCoreV2S1!, CancellationToken.None);

        protected abstract Task DoTest(IForecastService service, CancellationToken cancellationToken);
    }
}
