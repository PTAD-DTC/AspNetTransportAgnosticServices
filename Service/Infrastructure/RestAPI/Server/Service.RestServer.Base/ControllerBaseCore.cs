using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.Base;

namespace Service.RestServer.Base
{
    /// <summary>
    /// Base class for WeatherService controllers 
    /// </summary>
    public abstract class ControllerBaseCore<TService> : ControllerBase
    where TService : class
    {
        private readonly TService _service;
        private readonly string _apiVersion;

        /// <summary>
        /// ctor
        /// </summary>
        protected ControllerBaseCore(TService service, string apiVersion = null)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _apiVersion = apiVersion;
        }

        /// <summary>
        /// Converts common status results
        /// </summary>
        /// <param name="serviceCallResult"></param>
        /// <returns></returns>
        protected ActionResult<T> ConvertCommonActionResult<T>(CallResult<T> serviceCallResult) =>
            serviceCallResult?.ResultCode switch
            {
                ResultCode.Ok => !EqualityComparer<T>.Default.Equals(serviceCallResult.Result, default)
                    ? Ok(serviceCallResult.Result)
                    : StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected serviceResult conversion error"),
                ResultCode.NotFound => NotFound(),
                ResultCode.BadRequest => BadRequest(),
                ResultCode.Error => StatusCode((int)HttpStatusCode.InternalServerError, serviceCallResult.Description),
                _ => StatusCode((int)HttpStatusCode.InternalServerError, $"Unrecognized service result code {serviceCallResult?.ResultCode}: {serviceCallResult?.Description}")
            };

        /// <summary>
        /// Adding conditional debug api-version info to response header
        /// </summary>
        [Conditional("DEBUG")]
        private void AddApiVersionHeader()
        {
            if (!string.IsNullOrEmpty(_apiVersion))
            {
                Response.Headers.Add("Debug-Api-Version", _apiVersion);
            }
        }

        /// <summary>
        /// Processes service call and produces controller response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCall"></param>
        /// <param name="resultConvert"></param>
        /// <returns></returns>
        protected async Task<ActionResult<T>> CallService<T>(Func<TService, Task<CallResult<T>>> serviceCall, Func<CallResult<T>, ActionResult<T>> resultConvert)
        {
            if (serviceCall is null)
            {
                throw new ArgumentNullException(nameof(serviceCall));
            }
            if (resultConvert is null)
            {
                throw new ArgumentNullException(nameof(resultConvert));
            }

            AddApiVersionHeader();

            try
            {
                var serviceResult = await serviceCall(_service);
                return resultConvert(serviceResult);
            }
            catch (NotImplementedException)
            {
                return StatusCode((int)HttpStatusCode.NotImplemented);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, $"{e.GetType().Name}: {e.Message}");
            }
        }

        /// <summary>
        /// Processes service call and produces controller response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCall"></param>
        /// <returns></returns>
        protected Task<ActionResult<T>> CallService<T>(Func<TService, Task<CallResult<T>>> serviceCall) =>
            CallService(serviceCall, ConvertCommonActionResult);
    }
}
