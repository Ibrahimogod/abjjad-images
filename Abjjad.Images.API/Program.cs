using Abjjad.Images;
using Abjjad.Images.Factories;
using Abjjad.Images.API.Factories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Abjjad.Images.API.HealthChecks;
using Abjjad.Images.API.Middleware;
using Abjjad.Images.Core.Models;
using HealthChecks.UI.Client;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddCheck<ImagesDirectoryHealthCheck>("images-directory")
    .AddCheck<DataFileHealthCheck>("data-file");

builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(5);
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.SetApiMaxActiveRequests(1);
    //TODO: Make port host mapping dynamic
    var port = builder.Configuration.GetValue<int>("ASPNETCORE_HTTP_PORTS");
    options.AddHealthCheckEndpoint("Self", $"http://localhost:{port}/healthz");
})
.AddInMemoryStorage();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Abjjad Images API", 
        Version = "v1",
        Description = "Image processing API service for Abjjad platform"
    });
});

builder.Services
    .AddSingleton<IJsonFileStorageFactory<Enhancement, Guid>, DefaultJsonFileStorageFactory<Enhancement, Guid>>()
    .AddSingleton<IImageFileStorageFactory, DefaultImageFileStorageFactory>();

builder.Services.AddAbjjadImagesServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthz-ui";
    options.ApiPath = "/healthz-api";
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

await app.RunAsync();