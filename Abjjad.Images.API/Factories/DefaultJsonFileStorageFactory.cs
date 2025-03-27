using Abjjad.Images.Core;
using Abjjad.Images.Factories;
using Abjjad.Images.Storage;

namespace Abjjad.Images.API.Factories;

public class DefaultJsonFileStorageFactory<TEntity, TId> : IJsonFileStorageFactory<TEntity,TId> 
    where TEntity : class, IEntity<TId>, new() 
    where TId : notnull
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public DefaultJsonFileStorageFactory(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    
    public JsonFileStorage<TEntity, TId> Create(string filePath)
    {
        var fileStoragePath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);
        return new JsonFileStorage<TEntity, TId>(fileStoragePath);
    }
}