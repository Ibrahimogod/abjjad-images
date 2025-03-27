using Abjjad.Images.Core.Enums;
using Abjjad.Images.Models;
using SixLabors.ImageSharp;

namespace Abjjad.Images.Utils;

public interface IImageProcessor
{
    Task<ProcessedImageResult> ProcessImageAsync(Stream imageStream, ImageSize size, CancellationToken cancellationToken);
    Size GetDimensionsForSize(ImageSize size);
}