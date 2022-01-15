using ForecastService.RestDefinitions;
using ForecastService.RestExamples.V2_0;
using ForecastService.RestServerConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface.Base;
#if COMMENTSERVICECODE
using CommentService.RestServerConfiguration;
#endif
#if USESWAGGER
using Service.RestServer.Swagger;
#endif

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options => options.EnableEndpointRouting = false)
    .AddJsonOptions(options => SerializerOptions.SetDefaultOptions(options.JsonSerializerOptions));

builder.Services
    .AddVersionedApiExplorer(options =>
    {
        // formats: https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    })
    .AddApiVersioning(options =>
    {
#if USESWAGGER
        if (Definitions.DefaultApiVersion is { })
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = Definitions.DefaultApiVersion;
        }
#endif
    });


builder.Services
    .UseForecastService();
#if COMMENTSERVICECODE
builder.Services
    .UseCommentService();
#else
builder.Services
    .UseCommentServiceNativeClient(new System.Uri(builder.Configuration["CommentServiceUrl"] ?? "https://localhost:6011"));
#endif

#if USESWAGGER
builder.Services
    .AddSwagger(Definitions.ServiceName, Definitions.DefaultApiVersion, typeof(ForecastCommentDataDtoExample).Assembly);
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

app
    .UseRouting()
    .UsePathBase(Definitions.ServicePathBase)
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    })
    .UseMvc();

#if USESWAGGER
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger(provider, Definitions.ServicePathBase, Definitions.ServiceName, Definitions.DefaultApiVersion);
#endif

app.Run();