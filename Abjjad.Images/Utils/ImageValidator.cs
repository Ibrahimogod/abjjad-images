using Abjjad.Images.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Abjjad.Images.Utils;

public class ImageValidator : IImageValidator
{
    private readonly ValidationOptions _validationOptions;

    public ImageValidator(IOptions<ValidationOptions> validationOptions)
    {
        _validationOptions = validationOptions.Value;
    }

    public IEnumerable<string> ValidateImages(IEnumerable<IFormFile> images)
    {
        var errors = new List<string>();

        foreach (var image in images)
        {
            if (image.Length > _validationOptions.MaxFileSize)
            {
                errors.Add($"File '{image.FileName}' size is too large. Maximum size is {_validationOptions.MaxFileSize / (1024 * 1024)}MB");
            }

            if (!_validationOptions.AllowedContentTypes.Contains(image.ContentType))
            {
                errors.Add($"File '{image.FileName}' type is not supported. Supported types are: {string.Join(", ", _validationOptions.AllowedContentTypes)}");
            }

            if (!_validationOptions.AllowedExtensions.Contains(Path.GetExtension(image.FileName).ToLower()))
            {
                errors.Add($"File '{image.FileName}' extension is not supported. Supported extensions are: {string.Join(", ", _validationOptions.AllowedExtensions)}");
            }
        }

        return errors;
    }
}