namespace Abjjad.Images.Models;

public record ImageUploadResponse
{
    public IEnumerable<Guid> ImageIds { get; set; }
}