using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;
using Abjjad.Images.Models;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Managers;

public interface IImagesManager
{
    Dictionary<string, string>? GetMetadata(Guid id);
    IEnumerable<Enhancement>? GetAllEnhancements();
    Task<ImagesUploadResponse> ResizeImagesAsync(IEnumerable<IFormFile> files, string requestId, CancellationToken cancellationToken);
    ImagePath? GetImagePath(Guid id, ImageSize imageSize);
}