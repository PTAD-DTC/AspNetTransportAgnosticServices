using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Service.RestServer.Swagger
{
    internal sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly SwaggerConfigurationData _configurationData;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, SwaggerConfigurationData configurationData)
        {
            _provider = provider;
            _configurationData = configurationData;
        }

        public void Configure(SwaggerGenOptions options) =>
            options.Configure(_provider, _configurationData.ServiceName, _configurationData.DefaultApiVersion);
    }
}
