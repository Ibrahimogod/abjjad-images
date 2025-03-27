﻿using Abjjad.Images.Models;
using SixLabors.ImageSharp;
using Abjjad.Images.Options;
using Abjjad.Images.Core.Enums;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace Abjjad.Images.Utils;

public class ImageProcessor
{
    private readonly ImageProcessingOptions _imageProcessingOptions;

    public ImageProcessor(IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    public async Task<ProcessedImageResult> ProcessImageAsync(Stream imageStream, ImageSize size, CancellationToken cancellationToken)
    {
        var dimensions = GetDimensionsForSize(size);
        if (imageStream.CanSeek)
        {
            imageStream.Position = 0;
        }

        var outputStream = new MemoryStream();
        using (var image = await Image.LoadAsync(imageStream, cancellationToken))
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = dimensions,
                Mode = ResizeMode.Max
            }));

            await image.SaveAsync(outputStream, new WebpEncoder
            {
                Quality = _imageProcessingOptions.Quality,
                Method = (WebpEncodingMethod)_imageProcessingOptions.Method,
            }, cancellationToken);
        }

        outputStream.Position = 0;
        return new ProcessedImageResult
        {
            Stream = outputStream,
            Extension = ".webp",
            ContentType = "image/webp"
        };
    }

    private Size GetDimensionsForSize(ImageSize size) => size switch
    {
        ImageSize.Phone => new Size(_imageProcessingOptions.PhoneWidth, _imageProcessingOptions.PhoneHeight),
        ImageSize.Tablet => new Size(_imageProcessingOptions.TabletWidth, _imageProcessingOptions.TabletHeight),
        ImageSize.Desktop => new Size(_imageProcessingOptions.DesktopWidth, _imageProcessingOptions.DesktopHeight),
        _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
    };
}