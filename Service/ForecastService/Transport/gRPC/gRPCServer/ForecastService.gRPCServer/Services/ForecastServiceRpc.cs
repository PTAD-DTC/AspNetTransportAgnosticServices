using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Service.Interface.Base;

namespace ForecastService.gRPCServer.Services
{
    public class ForecastServiceRpc : Forecast.ForecastBase
	{
		private readonly IForecastService _weatherService;

		public ForecastServiceRpc(IForecastService weatherService)
		{
			_weatherService = weatherService;
		}

		public override async Task<GetTodayForecastsReply> GetTodayForecasts(GetTodayForecastsRequest request, ServerCallContext context)
		{
			var callResult = await _weatherService.GetTodayForecasts(context.CancellationToken);

			if (callResult.ResultCode != ResultCode.Ok)
			{
				return new GetTodayForecastsReply();
			}

			var query =
				from f in callResult.Result
				select new WeatherForecast()
				{
					Id = f.Id.ToString(),
					Celsius = f.TemperatureC,
					Fahrenheit = f.TemperatureF,
					Date = f.Date.ToTimestamp(),
					Description = f.Description,
					Location = f.Location,
					Probability = f.Probability switch
					{
						ForecastProbabilityDto.Guaranteed => WeatherForecast.Types.Probability.Guaranteed,
						ForecastProbabilityDto.High => WeatherForecast.Types.Probability.High,
						ForecastProbabilityDto.Low => WeatherForecast.Types.Probability.Low,
						_ => WeatherForecast.Types.Probability.Notdefined
					},
					Summary = f.Summary
				};

			var forecasts = new RepeatedField<WeatherForecast>();
			forecasts.AddRange(query);
			return new GetTodayForecastsReply()
			{
				Forecasts = { forecasts }
			};
		}
	}
}
