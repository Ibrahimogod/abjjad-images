using Abjjad.Images.Core;
using Abjjad.Images.Factories;
using Abjjad.Images.Options;
using Abjjad.Images.Storage;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Abjjad.Images.Extensions;

public static class FileStorageExtensions
{
    public static IServiceCollection AddJsonFileStorage<TEntity, TId>(this IServiceCollection services)
        where TEntity : class, IEntity<TId>, new() where TId : notnull
    {
        return 
            services
                .AddSingleton(sp =>
                {
                    IJsonFileStorageFactory<TEntity, TId> factory = sp.GetRequiredService<IJsonFileStorageFactory<TEntity, TId>>();
                    return factory.Create(sp.GetRequiredService<IOptions<StorageOptions>>().Value.BaseFilesPath);
                })
                .AddHostedService(sp => sp.GetRequiredService<JsonFileStorage<TEntity, TId>>());
    }

    public static IServiceCollection AddImageFileStorage(this IServiceCollection services) => services.AddSingleton(sp =>
    {
        IImageFileStorageFactory factory = sp.GetRequiredService<IImageFileStorageFactory>();
        var filePath = sp.GetRequiredService<IOptions<StorageOptions>>().Value.BaseImagesPath;
        return factory.Create(filePath);
    });
}