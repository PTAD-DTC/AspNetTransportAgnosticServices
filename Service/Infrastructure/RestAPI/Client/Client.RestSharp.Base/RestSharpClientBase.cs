using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Base;
using JetBrains.Annotations;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;

namespace Client.RestSharp.Base
{
    /// <summary>
    /// Base class for Clients that contact other services via Rest Client.
    /// </summary>
    [PublicAPI]
    public abstract class RestSharpClientBase: ClientBase
    {
        /// <summary>
        /// Internal rest client
        /// </summary>
        private readonly Lazy<IRestClient> _restClient;

        /// <summary>
        /// Constructor
        /// </summary>
        protected RestSharpClientBase(IRestClient restClient, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : this(GetClientCheck(restClient).BaseUrl, apiVersion, communicationListener)
        {
            if (restClient is null)
            {
                throw new ArgumentNullException(nameof(restClient));
            }

            _restClient = new Lazy<IRestClient>(restClient);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected RestSharpClientBase(Uri? serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(serviceBaseUrl, apiVersion, communicationListener)
        {
            _restClient = new Lazy<IRestClient>(PrepareDefaultRestClient);
        }

        /// <summary>
        /// Internal rest client
        /// </summary>
        private IRestClient RestClient => _restClient.Value;

        private static IRestClient GetClientCheck(IRestClient restClient)
        {
            return restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        /// <summary>
        /// Adding conditional debug api-version info to response header
        /// </summary>
        [Conditional("DEBUG")]
        private void AddApiVersionHeader(IRestRequest request)
        {
            if (!string.IsNullOrEmpty(ApiVersion))
            {
                request.AddHeader("Debug-Api-Version", ApiVersion);
            }
        }

        private async Task<IRestResponse> Request<TRequestContent>(IRestRequest request, TRequestContent requestContent, CancellationToken cancellationToken)
            where TRequestContent : class
        {
            AddApiVersionHeader(request);
            if (!string.IsNullOrEmpty(ApiVersion))
            {
                request.AddParameter("api-version", ApiVersion, ParameterType.QueryString);
            }

            if (requestContent is { })
            {
                request.AddJsonBody(requestContent);
            }

            if (CommunicationListener is { })
            {
                await CommunicationListener.OnRequestSend(
                    request.Method.ToString(),
                    $"{ServiceBaseUrl}{request.Resource}",
                    GetInfoForListener(request),
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    Serialize(request.Body?.Value),
                    cancellationToken);
            }

            var result = await RestClient.ExecuteAsync(request, cancellationToken);
            return result;
        }

        // ReSharper disable once StringLiteralTypo
        protected override string UserAgentName => "svc-restsharp-client";

        protected static IReadOnlyCollection<string> GetInfoForListener(IEnumerable<Parameter> data)
        {
            return data.Select(p => $"{p.Name}: {p.Value}").ToArray();
        }

        protected static IReadOnlyCollection<string> GetInfoForListener(IRestRequest request)
        {
            return GetInfoForListener(request?.Parameters ?? Enumerable.Empty<Parameter>());
        }

        protected static IReadOnlyCollection<string> GetInfoForListener(IRestResponse response)
        {
            return GetInfoForListener(response?.Headers ?? Enumerable.Empty<Parameter>());
        }

        protected virtual IRestClient PrepareDefaultRestClient()
        {
            var client = new RestClient(ServiceBaseUrl)
            {
                ThrowOnAnyError = true,
            };
            var serializerOptions = PrepareDefaultSerializerOptions();
            client.UseSystemTextJson(serializerOptions);
            client.UserAgent = UserAgentName;

            return client;
        }

        protected Task<IRestResponse> Request<TRequestContent>(Method method, string resourceRelativeUrl, TRequestContent requestContent, CancellationToken cancellationToken)
            where TRequestContent : class
        {
            return Request(new RestRequest(resourceRelativeUrl, method), requestContent, cancellationToken);
        }
    }
}
