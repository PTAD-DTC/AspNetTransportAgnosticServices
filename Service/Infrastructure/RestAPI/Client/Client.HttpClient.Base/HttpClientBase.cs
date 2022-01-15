using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Base;
using JetBrains.Annotations;

namespace Client.HttpClient.Base
{
    /// <summary>
    /// Base class for Clients that contact other services via HTTP.
    /// </summary>
    [PublicAPI]
    public abstract class HttpClientBase: ClientBase, IDisposable
    {
        private bool disposedValue;
        /// <summary>
        /// Internal http client
        /// </summary>
        private readonly Lazy<System.Net.Http.HttpClient> _httpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        protected HttpClientBase(System.Net.Http.HttpClient httpClient, Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
        : base(serviceBaseUrl, apiVersion, communicationListener)
        {
            if (httpClient is null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            _httpClient = new Lazy<System.Net.Http.HttpClient>(httpClient);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected HttpClientBase(Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : this(new System.Net.Http.HttpClient(), serviceBaseUrl, apiVersion, communicationListener)
        {
        }

        /// <summary>
        /// Internal rest client
        /// </summary>
        private System.Net.Http.HttpClient HttpClient => _httpClient.Value;

        /// <summary>
        /// Adding conditional debug api-version info to response header
        /// </summary>
        [Conditional("DEBUG")]
        private void AddApiVersionHeader(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(ApiVersion))
            {
                request.Headers.Add("Debug-Api-Version", ApiVersion);
            }
        }

        private HttpRequestMessage PrepareMessage(HttpMethod httpMethod, string relativeUrl)
        {
            var apiVersionParameter = !string.IsNullOrEmpty(ApiVersion)
                ? $"?api-version={ApiVersion}"
                : null;

            var requestUrl = new Uri(ServiceBaseUrl, $"{relativeUrl}{apiVersionParameter}");
            var requestMessage = new HttpRequestMessage(httpMethod, requestUrl);
            requestMessage.Headers.Add("User-Agent", UserAgentName);
            AddApiVersionHeader(requestMessage);
            return requestMessage;
        }

        protected override string UserAgentName => "svc-http-client";

        protected static IReadOnlyCollection<string> GetHeadersForListener(HttpHeaders headers)
        {
            return (headers?.SelectMany(h => h.Value.Select(v => $"{h.Key}: {v}")) ?? Enumerable.Empty<string>()).ToArray();
        }

        /// <summary>
        /// Sends a HTTP POST request to a specified URL with a specified payload and returns a raw response.
        /// </summary>
        protected async Task<HttpResponseMessage> SendRequest<TRequestContent>(HttpMethod httpMethod, string relativeUrl, TRequestContent requestContent,
            CancellationToken cancellationToken)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var requestMessage = PrepareMessage(httpMethod, relativeUrl);

            if (!EqualityComparer<TRequestContent>.Default.Equals(requestContent, default))
            {
                requestMessage.Content = new StringContent(
                        Serialize(requestContent),
                        Encoding.UTF8,
                        "application/json");
            }

            if (CommunicationListener is { })
            {
                await CommunicationListener.OnRequestSend(
                    requestMessage.Method?.Method,
                    requestMessage.RequestUri?.ToString(),
                    GetHeadersForListener(requestMessage.Headers),
                    Serialize(requestContent),
                    cancellationToken);
            }

            return await HttpClient.SendAsync(requestMessage, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)

                    if (_httpClient.IsValueCreated)
                    {
                        _httpClient.Value.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
