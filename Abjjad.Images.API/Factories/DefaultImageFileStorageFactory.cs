using Abjjad.Images.Factories;
using Abjjad.Images.Storage;

namespace Abjjad.Images.API.Factories;

public class DefaultImageFileStorageFactory : IImageFileStorageFactory
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public DefaultImageFileStorageFactory(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    
    public IImageFileStorage Create(string fileStoragePath)
    {
        var storagePath = Path.Combine(_webHostEnvironment.WebRootPath, fileStoragePath);
        return new ImageFileStorage(storagePath);
    }
}