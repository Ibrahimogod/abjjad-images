using Abjjad.Images;
using Abjjad.Images.Factories;
using Abjjad.Images.Core.Models;
using Abjjad.Images.API.Factories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<IJsonFileStorageFactory<Enhancement, Guid>, DefaultJsonFileStorageFactory<Enhancement, Guid>>()
    .AddSingleton<IImageFileStorageFactory, DefaultImageFileStorageFactory>();

builder.Services.AddAbjjadImagesServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();