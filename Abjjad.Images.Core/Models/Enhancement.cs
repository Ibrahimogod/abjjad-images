using Abjjad.Images.Core.Enums;

namespace Abjjad.Images.Core.Models;

public class Enhancement : Entity<Guid>
{
    public string RequestId { get; set; }
    public ImagePath OriginalImagePath { get; set; }
    public Dictionary<ImageSize, ImagePath> ResizedImagePaths { get; set; } = new Dictionary<ImageSize, ImagePath>();
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}