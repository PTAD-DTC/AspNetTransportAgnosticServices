using Microsoft.AspNetCore.Mvc;

namespace Service.RestServer.Swagger
{
    internal sealed class SwaggerConfigurationData
    {
        public SwaggerConfigurationData(string serviceName, ApiVersion? defaultApiVersion)
        {
            ServiceName = serviceName;
            DefaultApiVersion = defaultApiVersion;
        }

        public string ServiceName { get; }
        public ApiVersion? DefaultApiVersion { get; }
    }
}
