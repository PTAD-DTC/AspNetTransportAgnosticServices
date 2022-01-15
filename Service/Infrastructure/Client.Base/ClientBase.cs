using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Client.Base
{
    /// <summary>
    /// Base class for service clients
    /// </summary>
    [PublicAPI]
    public abstract class ClientBase
    {
        /// <summary>
        /// Serialization options
        /// </summary>
        private readonly Lazy<JsonSerializerOptions?> _defaultJsonSerializerOptions;

        protected ClientBase(Uri? serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
        {
            _defaultJsonSerializerOptions = new Lazy<JsonSerializerOptions?>(PrepareDefaultSerializerOptions);
            ServiceBaseUrl = AssureValidServiceBaseUrl(serviceBaseUrl);
            ApiVersion = apiVersion;
            SetCommunicationListener(communicationListener);
        }

        [Conditional("DEBUG")]
        private static void SetSerializerDebugOptions(JsonSerializerOptions jsonSerializerOptions) =>
            jsonSerializerOptions.WriteIndented = true;

        [Conditional("DEBUG")]
        private void SetCommunicationListener(ICommunicationDebugListener? communicationListener) =>
            CommunicationListener = communicationListener;

        protected Uri ServiceBaseUrl { get; }

        protected ICommunicationDebugListener? CommunicationListener { get; private set; }

        /// <summary>
        /// Serialization options
        /// </summary>
        protected JsonSerializerOptions? JsonSerializerOption => _defaultJsonSerializerOptions.Value;

        protected virtual string UserAgentName => "svc-client";

        protected static Uri AssureValidServiceBaseUrl(Uri? serviceBaseUrl)
        {
            if (serviceBaseUrl is null)
            {
                throw new ArgumentNullException(nameof(serviceBaseUrl));
            }

            try
            {
                if (serviceBaseUrl.IsAbsoluteUri == false)
                    throw new ArgumentException("The Base URL must be an absolute URL.", nameof(serviceBaseUrl));
                if (!string.IsNullOrEmpty(serviceBaseUrl.Query))
                    throw new ArgumentException("The query string part is not allowed in the base url.", nameof(serviceBaseUrl));
                if (!string.IsNullOrEmpty(serviceBaseUrl.Fragment))
                    throw new ArgumentException("The fragment (anchor) part is not allowed in the base url.", nameof(serviceBaseUrl));
            }
            catch (UriFormatException fe)
            {
                throw new ArgumentException("The provided Base URL is not a valid URL.", fe);
            }

            return serviceBaseUrl;
        }

        protected string Serialize<TValue>(TValue value) =>
            JsonSerializer.Serialize(value, JsonSerializerOption);

        protected TValue? Deserialize<TValue>(string json) =>
            JsonSerializer.Deserialize<TValue>(json, JsonSerializerOption);

        protected virtual JsonSerializerOptions PrepareDefaultSerializerOptions()
        {
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(jsonSerializerOptions.PropertyNamingPolicy));
            SetSerializerDebugOptions(jsonSerializerOptions);
            return jsonSerializerOptions;
        }

        public string ApiVersion { get; }
    }
}
