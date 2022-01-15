﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using Microsoft.AspNetCore.Mvc;
#if USESWAGGER
using System.Net;
using Swashbuckle.AspNetCore.Annotations;
#endif

namespace ForecastService.RestServer.V2_1.Controllers
{
    /// <summary>
    /// Controller responsible for weather forecasts
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("2.1")]
    public class ForecastController : ControllerBaseForecastService
    {
        // SwaggerResponse with BadRequest makes interface generated by AutoRest very ugly (response with object - no strong type in interface)

        /// <summary>
        /// ctor
        /// </summary>
        public ForecastController(IForecastService weatherService) : base(weatherService)
        {
        }

        /// <summary>
        /// Returns today's forecasts for all locations
        /// </summary>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        [HttpGet("today")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Forecasts are returned", typeof(IEnumerable<WeatherForecastDto>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "No forecasts")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public Task<ActionResult<IReadOnlyCollection<WeatherForecastDto>?>> GetTodayForecasts(CancellationToken cancellationToken)
        {
            return CallService(svc => svc.GetTodayForecasts(cancellationToken));
        }

        /// <summary>
        /// Returns forecasts for specified location
        /// </summary>
        /// <param name="location">location name</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        [HttpGet("location/{location}")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Forecasts are returned", typeof(IEnumerable<WeatherForecastDto>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Location not found")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public Task<ActionResult<IReadOnlyCollection<WeatherForecastDto>?>> GetLocationForecasts(string location, CancellationToken cancellationToken)
        {
            return CallService(svc => svc.GetLocationForecasts(location, cancellationToken));
        }

        /// <summary>
        /// Returns specified forecast
        /// </summary>
        /// <param name="forecastId"></param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        [HttpGet("{forecastId}")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Forecast is returned", typeof(WeatherForecastDto))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Forecast not found")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public Task<ActionResult<WeatherForecastDto?>> GetForecast(Guid forecastId, CancellationToken cancellationToken)
        {
            return CallService(svc => svc.FindForecast(forecastId, cancellationToken));
        }
    }
}