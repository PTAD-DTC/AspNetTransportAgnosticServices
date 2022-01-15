using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client.Base;
using Client.HttpClient.Base;
using JetBrains.Annotations;
using Service.Interface.Base;
using ServiceHelpers;

namespace ServiceBaseClient.HttpClient.Base
{
    /// <summary>
    /// Base class for Clients that contact other services via HTTP.
    /// </summary>
    [PublicAPI]
    public abstract class ServiceHttpClientBase : HttpClientBase, IServiceBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ServiceHttpClientBase(System.Net.Http.HttpClient httpClient, Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(httpClient, serviceBaseUrl,  apiVersion, communicationListener)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected ServiceHttpClientBase(Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : this(new System.Net.Http.HttpClient(), serviceBaseUrl, apiVersion, communicationListener)
        {
        }

        protected abstract string ResponseApiVersion { get; }

        protected override JsonSerializerOptions PrepareDefaultSerializerOptions()
        {
            return SerializerOptions.GetDefaultOptions();
        }

        protected async Task<CallResult<TResult>> CallServiceSafe<TResult>(
            Func<CancellationToken, Task<HttpResponseMessage>> serviceCall,
            Func<string, CallResult<TResult>> successConvert,
            Func<HttpResponseMessage, CallResult<TResult>> failureConvert,
            Func<Exception, CallResult<TResult>> exceptionConvert,
            CancellationToken cancellationToken)
        {
            if (serviceCall is null)
            {
                throw new ArgumentNullException(nameof(serviceCall));
            }
            if (successConvert is null)
            {
                throw new ArgumentNullException(nameof(successConvert));
            }
            if (failureConvert is null)
            {
                throw new ArgumentNullException(nameof(failureConvert));
            }
            if (exceptionConvert is null)
            {
                throw new ArgumentNullException(nameof(exceptionConvert));
            }

            try
            {
                var serviceResponse = await serviceCall(cancellationToken);

                if (serviceResponse is null)
                {
                    return new CallResult<TResult>(ResultCode.Error, ResponseApiVersion, $"{nameof(serviceResponse)} is null");
                }

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var resultContent = await serviceResponse.Content.ReadAsStringAsync();
                    if (CommunicationListener is { })
                    {
                        await CommunicationListener.OnResponseReceived(
                            serviceResponse.RequestMessage?.Method.Method,
                            serviceResponse.RequestMessage?.RequestUri?.ToString(),
                            GetHeadersForListener(serviceResponse.Headers),
                            $"{(int?)serviceResponse.StatusCode} {serviceResponse.StatusCode}: {resultContent}",
                            cancellationToken);
                    }

                    var successResult = successConvert(resultContent);
                    return successResult;
                }

                if (CommunicationListener is { })
                {
                    await CommunicationListener.OnResponseReceived(
                        serviceResponse.RequestMessage?.Method.Method,
                        serviceResponse.RequestMessage?.RequestUri?.ToString(),
                        GetHeadersForListener(serviceResponse.Headers),
                        $"{(int?)serviceResponse.StatusCode} {serviceResponse.StatusCode}: {serviceResponse.ReasonPhrase}",
                        cancellationToken);
                }

                var failureResult = failureConvert(serviceResponse);
                return failureResult;
            }
            catch (Exception e)
            {
                return exceptionConvert(e);
            }
        }

        protected Task<CallResult<TResult?>> CallServiceSafe<TResult>(
            Func<CancellationToken, Task<HttpResponseMessage>> serviceCall,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe(
                serviceCall,
                content => new CallResult<TResult?>(Deserialize<TResult>(content), ResponseApiVersion),
                serviceResponse => new CallResult<TResult?>(serviceResponse.StatusCode.MapToResultCode(), ResponseApiVersion, serviceResponse.ReasonPhrase),
                e => new CallResult<TResult?>(ResultCode.Error, ResponseApiVersion, e.Message),
                cancellationToken
            );
        }

        protected Task<CallResult<TResult?>> CallGetServiceSafe<TResult>(
            string relativeUrl,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe<TResult>(
                ct => SendRequest(HttpMethod.Get, relativeUrl, default(object), ct),
                cancellationToken);
        }

        protected Task<CallResult<TResult?>> CallPostServiceSafe<TResult, TRequestContent>(
            string relativeUrl, TRequestContent requestContent,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe<TResult>(
                ct => SendRequest(HttpMethod.Post, relativeUrl, requestContent, ct),
                cancellationToken);
        }

        protected Task<CallResult<TResult?>> CallPutServiceSafe<TResult, TRequestContent>(
            string relativeUrl, TRequestContent requestContent,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe<TResult>(
                ct => SendRequest(HttpMethod.Put, relativeUrl, requestContent, ct),
                cancellationToken);
        }

        protected Task<CallResult<TResult?>> CallDeleteServiceSafe<TResult>(
            string relativeUrl,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe<TResult>(
                ct => SendRequest(HttpMethod.Delete, relativeUrl, default(object), ct),
                cancellationToken);
        }
    }
}
