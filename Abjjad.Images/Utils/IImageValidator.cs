using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Utils;

public interface IImageValidator
{
    IEnumerable<string> ValidateImages(IEnumerable<IFormFile> images);
}