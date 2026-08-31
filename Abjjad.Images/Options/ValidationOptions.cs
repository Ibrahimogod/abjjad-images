namespace Abjjad.Images.Options;

public class ValidationOptions
{
    public int MaxFileSize { get; set; } = 2 * 1024 * 1024; // Default: 2MB
    public string[] AllowedContentTypes { get; set; } = { "image/jpg", "image/jpeg", "image/png", "image/webp" };
    public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
}