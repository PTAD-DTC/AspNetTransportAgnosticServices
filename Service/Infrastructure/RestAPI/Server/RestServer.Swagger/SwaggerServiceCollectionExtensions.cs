using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Service.RestServer.Swagger
{
    public static class SwaggerServiceCollectionExtensions
    {
        private const string DefaultDocName = "default";
        private const string ApiVersionDocName = "apiversion";

        public static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName, ApiVersion? defaultApiVersion, params Assembly[] exampleAssemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services
                .AddSingleton(_ => new SwaggerConfigurationData(serviceName, defaultApiVersion))
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddSwaggerExamplesFromAssemblyOf<ConfigureSwaggerOptions>()
                .AddSwaggerGen();

            var callingAssembly = Assembly.GetCallingAssembly();
            services.AddSwaggerExamplesFromAssemblies(callingAssembly);
            if (exampleAssemblies is { Length: > 0 })
            {
                services.AddSwaggerExamplesFromAssemblies(exampleAssemblies);
            }

            return services;
        }

        public static void Configure(this SwaggerGenOptions options, IApiVersionDescriptionProvider versionProvider, string serviceName, ApiVersion? defaultApiVersion)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (versionProvider == null)
            {
                throw new ArgumentNullException(nameof(versionProvider));
            }

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                return docName switch
                {
                    DefaultDocName => defaultApiVersion is { } && apiDesc.GetApiVersion() == defaultApiVersion && (apiDesc.RelativePath?.StartsWith(apiDesc.GroupName ?? string.Empty) ?? false),
                    var dn when dn.StartsWith(ApiVersionDocName) => docName.EndsWith(apiDesc.GroupName ?? string.Empty) && !(apiDesc.RelativePath?.StartsWith(apiDesc.GroupName ?? string.Empty) ?? false),
                    _ => apiDesc.GroupName == docName && (apiDesc.RelativePath?.StartsWith(apiDesc.GroupName ?? string.Empty) ?? false)
                };
            });

            var defaultDescription = versionProvider.ApiVersionDescriptions.FirstOrDefault(d => d.ApiVersion == defaultApiVersion);
            if (defaultDescription != null)
            {
                options.SwaggerDoc(
                    DefaultDocName,
                    new OpenApiInfo
                    {
                        Title = $"{serviceName}",
                        Version = defaultDescription.ApiVersion.ToString()
                    });
            }

            foreach (var description in versionProvider.ApiVersionDescriptions)
                options.SwaggerDoc(
                    $"{description.GroupName}",
                    new OpenApiInfo
                    {
                        Title = $"{serviceName} {description.GroupName}",
                        Version = description.ApiVersion.ToString()
                    });

            foreach (var description in versionProvider.ApiVersionDescriptions)
                options.SwaggerDoc(
                    $"{ApiVersionDocName}{description.GroupName}",
                    new OpenApiInfo
                    {
                        Title = $"{serviceName} {description.GroupName}",
                        Version = description.ApiVersion.ToString()
                    });

            // Required for some client generators (ie. Viual Studio built in AutoRest)
            options.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
            options.EnableAnnotations();
            options.ExampleFilters();

            var modelsAssemblyName = Assembly.GetEntryAssembly()?.Location;
            var path = Path.ChangeExtension(modelsAssemblyName, "xml");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var xmlDocumentationFilePath = Path.Combine(AppContext.BaseDirectory, path);
            options.IncludeXmlComments(xmlDocumentationFilePath);
        }

        public static void UseSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, PathString servicePathBase, string serviceName, ApiVersion? defaultApiVersion)
        {
            var servicePathBaseUri = servicePathBase.ToUriComponent();
            var swaggerPathBase = $"swagger{servicePathBaseUri}";
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            _ = app.UseSwagger(options =>
                {
                    // Required for some client generators (ie. AutoRest)
                    //options.SerializeAsV2 = true;
                    options.RouteTemplate = $"{swaggerPathBase}/{{documentName}}/swagger.json";
                    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                        swaggerDoc.Paths = ExtractPaths(servicePathBaseUri, swaggerDoc));
                });

            // Enable middleware to serve Swagger-ui
            app.UseSwaggerUI(options =>
            {
                DefineEndpoints(options, provider, swaggerPathBase, serviceName, defaultApiVersion);
                options.RoutePrefix = $"{swaggerPathBase}";
            });

            static OpenApiPaths ExtractPaths(string serviceBasePath, OpenApiDocument swaggerDoc)
            {
                var modifiedPaths = from pair in swaggerDoc.Paths
                                    select ($"{serviceBasePath}{pair.Key}", pair.Value);
                modifiedPaths = modifiedPaths.ToArray();
                var swaggerPaths = new OpenApiPaths();
                foreach (var (key, value) in modifiedPaths)
                {
                    swaggerPaths.Add(key, value);
                }

                return swaggerPaths;
            }

            static void DefineEndpoints(SwaggerUIOptions options, IApiVersionDescriptionProvider provider, string swaggerPathBase, string serviceName, ApiVersion? defaultApiVersion)
            {
                if (defaultApiVersion is { })
                {
                    var defaultDescription = provider.ApiVersionDescriptions.FirstOrDefault(d => d.ApiVersion == defaultApiVersion);
                    if (defaultDescription != null)
                    {
                        options.SwaggerEndpoint(
                            $"/{swaggerPathBase}/{DefaultDocName}/swagger.json",
                            $"{serviceName} default [v{defaultApiVersion}]");
                    }
                }

                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint(
                        $"/{swaggerPathBase}/{ApiVersionDocName}{description.GroupName}/swagger.json",
                        $"{serviceName} [{description.GroupName}] {ApiVersionDocName}");

                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint(
                        $"/{swaggerPathBase}/{description.GroupName}/swagger.json",
                        $"{serviceName} [{description.GroupName}]");
            }
        }
    }
}
