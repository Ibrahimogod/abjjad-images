using Abjjad.Images.Utils;
using Abjjad.Images.Options;
using Abjjad.Images.Service;
using Abjjad.Images.Extensions;
using Abjjad.Images.Core.Models;
using Abjjad.Images.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abjjad.Images;

public static class Install
{
    public static IServiceCollection AddAbjjadImagesServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddUtils()
            .AddService()
            .AddManagers()
            .AddOptions(configuration)
            .AddJsonFileStorage<Enhancement, Guid>()
            .AddImageFileStorage();
    }

    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        return services
            .AddSingleton<IExifDataExtractor,ExifDataExtractor>()
            .AddSingleton<IImageProcessor, ImageProcessor>();
    }

    public static IServiceCollection AddService(this IServiceCollection services)
    {
       return services
            .AddSingleton<IImageService, ImageService>();
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        return services
            .AddSingleton<ImagesManager>();
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
       return services
            .Configure<StorageOptions>(configuration.GetSection(nameof(StorageOptions)))
            .Configure<ImageProcessingOptions>(configuration.GetSection(nameof(ImageProcessingOptions)));
    }
}