using Abjjad.Images.Core.Enums;

namespace Abjjad.Images.API.Models;

public record EnhancementModel
{
    public Guid Id { get; set; }
    public string RequestId { get; set; }
    public string OriginalImageUrl { get; set; }
    public Dictionary<ImageSize, string> ResizedImageUrls { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}