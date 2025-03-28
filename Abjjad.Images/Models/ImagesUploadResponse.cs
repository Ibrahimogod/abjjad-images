namespace Abjjad.Images.Models;

public record ImagesUploadResponse
{
    public IEnumerable<Guid> ImageIds { get; set; }
}