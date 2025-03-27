using Abjjad.Images.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Service;

public interface IImageService
{
    Task<Guid> ResizeImageAsync(IFormFile file, string requestId, CancellationToken cancellationToken);
    Enhancement? GetEnhancedImage(Guid id);
    IEnumerable<Enhancement>? GetAllEnhancements();
}