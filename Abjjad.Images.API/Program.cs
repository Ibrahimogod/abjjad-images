using Abjjad.Images;
using Abjjad.Images.Factories;
using Abjjad.Images.API.Factories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Abjjad.Images.API.HealthChecks;
using Abjjad.Images.Core.Models;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddCheck<ImagesDirectoryHealthCheck>("images-directory")
    .AddCheck<DataFileHealthCheck>("data-file");

builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(5);
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.SetApiMaxActiveRequests(1);
    options.AddHealthCheckEndpoint("Self", "/healthz");
})
.AddInMemoryStorage();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<IJsonFileStorageFactory<Enhancement, Guid>, DefaultJsonFileStorageFactory<Enhancement, Guid>>()
    .AddSingleton<IImageFileStorageFactory, DefaultImageFileStorageFactory>();

builder.Services.AddAbjjadImagesServices(builder.Configuration);

var app = builder.Build();

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

app.Run();