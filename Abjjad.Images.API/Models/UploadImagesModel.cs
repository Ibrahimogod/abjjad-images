using Microsoft.AspNetCore.Mvc;

namespace Abjjad.Images.API.Models;

public record UploadImagesModel
{
    [FromForm]
    public IEnumerable<IFormFile> Images { get; set; }  
}