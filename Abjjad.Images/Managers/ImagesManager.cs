using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;
using Abjjad.Images.Models;
using Abjjad.Images.Service;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Managers;

public class ImagesManager
{
    private readonly IImageService _imageService;
    
    public ImagesManager(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<ImagesUploadResponse> ResizeImagesAsync(IEnumerable<IFormFile> files, string requestId, CancellationToken cancellationToken)
    {
        var tasks = files.Select(f => ResizeSingleImageAsync(f, requestId, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return new ImagesUploadResponse { ImageIds = results };
    }
    
    private async Task<Guid> ResizeSingleImageAsync(IFormFile file, string requestId, CancellationToken cancellationToken)
    {
        return await _imageService.ResizeImageAsync(file, requestId, cancellationToken);
    }

    public ImagePath? GetImagePath(Guid id, ImageSize imageSize)
    {
        var enhancedImage = _imageService.GetEnhancedImage(id);
        return enhancedImage?.ResizedImagePaths.GetValueOrDefault(imageSize);
    }

    public Dictionary<string, string>? GetMetadata(Guid id)
    {
        var enhancedImage = _imageService.GetEnhancedImage(id);
        return enhancedImage?.Metadata;
    }

    public IEnumerable<Enhancement>? GetAllEnhancements()
    { 
        return _imageService.GetAllEnhancements();
    }
}