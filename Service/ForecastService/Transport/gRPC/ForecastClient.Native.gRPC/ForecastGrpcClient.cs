using ForecastService.gRPCClient;
using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using Grpc.Net.Client;
using Service.Interface.Base;

namespace ForecastClient.Native.gRPC
{
	public class ForecastGrpcClient : IForecastService, IDisposable
	{
		private bool _disposedValue;
		private readonly GrpcChannel _grpcChannel;
		private readonly Forecast.ForecastClient _client;

		public ForecastGrpcClient(Uri serviceUri)
		{
			_grpcChannel = GrpcChannel.ForAddress(serviceUri);
			_client = new Forecast.ForecastClient(_grpcChannel);
		}

		public string? ApiVersion { get; }

		public async Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetTodayForecasts(
            CancellationToken cancellationToken)
		{
			if (_disposedValue)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			var reply = await _client.GetTodayForecastsAsync(new GetTodayForecastsRequest());
			var query = from f in reply.Forecasts
						select new WeatherForecastDto(
							Guid.Parse(f.Id),
							f.Date.ToDateTime(),
							f.Location,
							f.Celsius,
							f.Fahrenheit,
							f.Summary,
							f.Description,
							f.Probability switch
							{
								WeatherForecast.Types.Probability.Guaranteed => ForecastProbabilityDto.Guaranteed,
								WeatherForecast.Types.Probability.High => ForecastProbabilityDto.High,
								WeatherForecast.Types.Probability.Low => ForecastProbabilityDto.Low,
								_ => ForecastProbabilityDto.NotDefined
							});

			return new CallResult<IReadOnlyCollection<WeatherForecastDto>?>(query.ToArray());
		}

        public Task<CallResult<WeatherForecastDto?>> FindForecast(
            Guid id, 
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetLocationForecasts(
            string location, 
            CancellationToken cancellationToken) => 
            throw new NotImplementedException();

        public Task<CallResult<IReadOnlyCollection<ForecastCommentDto>?>> GetForecastComments(
            Guid forecastId, 
            CancellationToken cancellationToken) => 
            throw new NotImplementedException();

        public Task<CallResult<ForecastCommentDto?>> AddComment(
            Guid forecastId, 
            ForecastCommentDataDto commentData, 
            CancellationToken cancellationToken) => 
            throw new NotImplementedException();

        public Task<CallResult<ForecastCommentDto?>> GetComment(
            Guid forecastId, 
            Guid commentId, 
            CancellationToken cancellationToken) => 
            throw new NotImplementedException();

        public Task<CallResult<ForecastCommentDto?>> UpdateComment(
            Guid forecastId, 
            Guid commentId, 
            ForecastCommentDataDto commentData, 
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue)
				return;

			if (disposing)
			{
				// dispose managed state (managed objects)

				_grpcChannel.Dispose();
			}

			// free unmanaged resources (unmanaged objects) and override finalizer
			// set large fields to null
			_disposedValue = true;
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
