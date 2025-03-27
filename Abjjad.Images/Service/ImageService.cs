using Abjjad.Images.Utils;
using Abjjad.Images.Storage;
using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Service;

public class ImageService : IImageService
{

    private readonly IImageProcessor _imageProcessor;
    private readonly IImageFileStorage _imageFileStorage;
    private readonly IExifDataExtractor _exifDataExtractor;
    private readonly IJsonFileStorage<Enhancement, Guid> _enhancementFileStorage;

    public ImageService(IImageFileStorage imageFileStorage, IExifDataExtractor exifDataExtractor, IJsonFileStorage<Enhancement, Guid> enhancementFileStorage, IImageProcessor imageProcessor)
    {
        _imageProcessor = imageProcessor;
        _imageFileStorage = imageFileStorage;
        _exifDataExtractor = exifDataExtractor;
        _enhancementFileStorage = enhancementFileStorage;
    }

    public async Task<Guid> ResizeImageAsync(IFormFile file, string requestId, CancellationToken cancellationToken)
    {
        var enhancement = new Enhancement
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
        };

        // Process original image and store it
        using var originalStream = new MemoryStream();
        await file.CopyToAsync(originalStream, cancellationToken);
        originalStream.Position = 0;

        // Store original image
        enhancement.OriginalImagePath = new ImagePath
        {
            Path = await _imageFileStorage.SaveAsync(
                directoryPath: requestId,
                fileName: $"{enhancement.Id}_original{Path.GetExtension(file.FileName)}",
                originalStream,
                cancellationToken),
            ContentType = file.ContentType
        };

        // Process and store resized versions in parallel
        var resizeTasks = new[]
        {
            ResizeImageAsync(requestId, enhancement.Id, originalStream, ImageSize.Phone, enhancement.ResizedImagePaths, cancellationToken),
            ResizeImageAsync(requestId, enhancement.Id, originalStream, ImageSize.Tablet, enhancement.ResizedImagePaths, cancellationToken),
            ResizeImageAsync(requestId, enhancement.Id, originalStream, ImageSize.Desktop, enhancement.ResizedImagePaths, cancellationToken)
        };

        // Extract metadata in parallel with resizing
        var metadataTask = _exifDataExtractor.ExtractExifDataAsync(originalStream, cancellationToken);

        // Wait for all operations to complete
        await Task.WhenAll(resizeTasks.Concat(new[] { metadataTask }));
        
        enhancement.Metadata = await metadataTask;
        _enhancementFileStorage.Add(enhancement);
            
        return enhancement.Id;
    }

    private async Task ResizeImageAsync(string requestId, Guid imageId, MemoryStream originalStream, ImageSize imageSize, Dictionary<ImageSize, ImagePath> resizedImagePaths, CancellationToken cancellationToken)
    {
        originalStream.Position = 0;
        var result = await _imageProcessor.ProcessImageAsync(originalStream, imageSize, cancellationToken);
        
        try
        {
            var imagePath = await _imageFileStorage.SaveAsync(
                directoryPath: requestId,
                fileName: $"{imageId}_{imageSize}{result.Extension}",
                stream: result.Stream,
                cancellationToken);
            resizedImagePaths.Add(imageSize, new ImagePath { Path = imagePath, ContentType = result.ContentType });
        }
        finally
        {
            result.Stream.Dispose();
        }
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