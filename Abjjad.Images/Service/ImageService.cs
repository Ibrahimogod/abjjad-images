using Abjjad.Images.Utils;
using Abjjad.Images.Storage;
using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Service;

public class ImageService
{

    private readonly ImageProcessor _imageProcessor;
    private readonly ImageFileStorage _imageFileStorage;
    private readonly ExifDataExtractor _exifDataExtractor;
    private readonly JsonFileStorage<Enhancement, Guid> _enhancementFileStorage;

    public ImageService(ImageFileStorage imageFileStorage, ExifDataExtractor exifDataExtractor, JsonFileStorage<Enhancement, Guid> enhancementFileStorage, ImageProcessor imageProcessor)
    {
        _imageProcessor = imageProcessor;
        _imageFileStorage = imageFileStorage;
        _exifDataExtractor = exifDataExtractor;
        _enhancementFileStorage = enhancementFileStorage;
    }

    public async Task<Guid> ResizeImageAsync(IFormFile file, string requestId, CancellationToken cancellationToken)
    {
        // Process original image
        using var originalStream = new MemoryStream();
        await file.CopyToAsync(originalStream, cancellationToken);
        originalStream.Position = 0;

        var enhancement = new Enhancement
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
        };
        
        // Store original
        enhancement.OriginalImagePath = new ImagePath
        {
            Path = await _imageFileStorage.SaveAsync(
                directoryPath: requestId,
                fileName: $"{enhancement.Id}_original{Path.GetExtension(file.FileName)}",
                originalStream,
                cancellationToken),
            ContentType = file.ContentType
        };
        var imageId = enhancement.Id;
        // Process and store resized versions
        var resizeTasks = new[]
        {
            ResizeImageAsync(requestId, imageId, originalStream, ImageSize.Phone, enhancement.ResizedImagePaths, cancellationToken),
            ResizeImageAsync(requestId, imageId, originalStream, ImageSize.Tablet, enhancement.ResizedImagePaths, cancellationToken),
            ResizeImageAsync(requestId, imageId, originalStream, ImageSize.Desktop, enhancement.ResizedImagePaths, cancellationToken)
        };

        await Task.WhenAll(resizeTasks);
        
        // Extract and store metadata
        enhancement.Metadata = await _exifDataExtractor.ExtractExifDataAsync(originalStream, cancellationToken);
        
        _enhancementFileStorage.Add(enhancement);
            
        return imageId;
    }

    private async Task ResizeImageAsync(string requestId, Guid imageId, MemoryStream originalStream, ImageSize imageSize, Dictionary<ImageSize, ImagePath> resizedImagePaths, CancellationToken cancellationToken)
    {
        var result = await _imageProcessor.ProcessImageAsync(originalStream, imageSize, cancellationToken);
        var imagePath = await _imageFileStorage.SaveAsync(
            directoryPath: requestId,
            fileName: $"{imageId}_{imageSize}{result.Extension}",
            stream: result.Stream,
            cancellationToken);
        resizedImagePaths.Add(imageSize, new ImagePath { Path = imagePath, ContentType = result.ContentType });
    }

    public Enhancement? GetEnhancedImage(Guid id)
    {
        return _enhancementFileStorage.GetById(id);
    }

    public IEnumerable<Enhancement>? GetAllEnhancements()
    {
        return _enhancementFileStorage.GetAll();
    }
}