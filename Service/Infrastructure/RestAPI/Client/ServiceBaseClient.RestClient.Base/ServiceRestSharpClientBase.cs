using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client.Base;
using Client.RestSharp.Base;
using JetBrains.Annotations;
using RestSharp;
using Service.Interface.Base;
using ServiceHelpers;

namespace ServiceBaseClient.RestClient.Base
{
    /// <summary>
    /// Base class for Clients that contact other services via Rest Client.
    /// </summary>
    [PublicAPI]
    public abstract class ServiceRestSharpClientBase : RestSharpClientBase, IServiceBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ServiceRestSharpClientBase(IRestClient restClient, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(restClient, apiVersion, communicationListener)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected ServiceRestSharpClientBase(Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(serviceBaseUrl, apiVersion, communicationListener)
        {
        }

        protected override JsonSerializerOptions PrepareDefaultSerializerOptions()
        {
            return SerializerOptions.GetDefaultOptions();
        }

        protected abstract string ResponseApiVersion { get; }

        protected async Task<CallResult<TResult>> CallServiceSafe<TResult>(
            Func<CancellationToken, Task<IRestResponse>> serviceCall,
            Func<TResult, CallResult<TResult>> successConvert,
            Func<IRestResponse, CallResult<TResult>> failureConvert,
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

                if (serviceResponse.IsSuccessful)
                {
                    if (CommunicationListener is { })
                    {
                        await CommunicationListener.OnResponseReceived(
                            serviceResponse.Request?.Method.ToString(),
                            serviceResponse.ResponseUri?.ToString(),
                            GetInfoForListener(serviceResponse),
                            $"{(int)serviceResponse.StatusCode} {serviceResponse.StatusCode}: {serviceResponse.Content}",
                            cancellationToken);
                    }

                    var data = Deserialize<TResult>(serviceResponse.Content);
                    var successResult = successConvert(data!);
                    return successResult;
                }

                if (CommunicationListener is { })
                {
                    await CommunicationListener.OnResponseReceived(
                        serviceResponse.Request?.Method.ToString(),
                        serviceResponse.ResponseUri?.ToString(),
                        GetInfoForListener(serviceResponse),
                        $"{serviceResponse.ResponseStatus} {(int)serviceResponse.StatusCode} {serviceResponse.StatusCode} {serviceResponse.ErrorMessage}",
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

        protected Task<CallResult<TResult>> CallServiceSafe<TResult>(
            Func<CancellationToken, Task<IRestResponse>> serviceCall,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe(
                serviceCall,
                content => new CallResult<TResult>(content, ResponseApiVersion),
                serviceResponse => new CallResult<TResult>(serviceResponse.StatusCode.MapToResultCode(), ResponseApiVersion, $"{serviceResponse.ResponseStatus}: {serviceResponse.ErrorMessage}"),
                e => new CallResult<TResult>(ResultCode.Error, ResponseApiVersion, e.Message),
                cancellationToken
            );
        }

        protected Task<CallResult<TResult>> CallGetServiceSafe<TResult>(
            string relativeUrl,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe<TResult>(
                ct => Request<object>(Method.GET, relativeUrl, default!, ct),
                cancellationToken);
        }

        protected Task<CallResult<TResult>> CallPostServiceSafe<TResult, TRequestContent>(
            string relativeUrl, TRequestContent requestContent,
            CancellationToken cancellationToken)
            where TRequestContent : class
        {
            return CallServiceSafe<TResult>(
                ct => Request(Method.POST, relativeUrl, requestContent, ct),
                cancellationToken);
        }

        protected Task<CallResult<TResult>> CallPutServiceSafe<TResult, TRequestContent>(
            string relativeUrl, TRequestContent requestContent,
            CancellationToken cancellationToken)
            where TRequestContent : class
        {
            return CallServiceSafe<TResult>(
                ct => Request(Method.PUT, relativeUrl, requestContent, ct),
                cancellationToken);
        }

        protected Task<CallResult<TResult>> CallDeleteServiceSafe<TResult>(
            string relativeUrl,
            CancellationToken cancellationToken)
        {
            return CallServiceSafe<TResult>(
                ct => Request<object>(Method.DELETE, relativeUrl, default!, ct),
                cancellationToken);
        }
    }
}
