using Abjjad.Images.Managers;
using Microsoft.AspNetCore.Mvc;
using Abjjad.Images.API.Models;
using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;

namespace Abjjad.Images.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private const string REQUIRED_CONTENT_TYPE = "multipart/form-data";
    private const int MAX_FILE_SIEZE = 2 * 1024 * 1024;
    private readonly string[] ALLOWED_CONTENT_TYPES = { "image/jpg", "image/jpeg", "image/png", "image/webp" };
    private readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };
    
    private readonly ImagesManager _imagesManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImagesController(ImagesManager imagesManager, IWebHostEnvironment webHostEnvironment)
    {
        _imagesManager = imagesManager;
        _webHostEnvironment = webHostEnvironment;
    }
    
    [HttpPost]
    [Consumes(REQUIRED_CONTENT_TYPE)]
    public async Task<IActionResult> Upload(UploadImagesModel model, CancellationToken cancellationToken)
    {
        foreach (var image in model.Images)
        {
            if (image.Length > MAX_FILE_SIEZE)
            {
                ModelState.AddModelError("Images", $"File '{image.FileName}' size is too large");
                return BadRequest(ModelState);
            }

            if (!ALLOWED_CONTENT_TYPES.Contains(image.ContentType))
            {
                ModelState.AddModelError("Images", $"File '{image.FileName}' type is not supported");
                return BadRequest(ModelState);
            }

            if (!ALLOWED_EXTENSIONS.Contains(Path.GetExtension(image.FileName).ToLower()))
            {
                ModelState.AddModelError("Images", $"File '{image.FileName}' extension is not supported");
                return BadRequest(ModelState);
            }
        }

        var images = await _imagesManager.ResizeImagesAsync(model.Images, HttpContext.TraceIdentifier.Replace(":", "_"), cancellationToken);
        return Ok(images);
    }

    [HttpGet]
    public IActionResult Get()
    {
        var enhancements = _imagesManager.GetAllEnhancements();
        var models = enhancements?.Select(GetEnhancementModel);
        return Ok(models);
    }

    [HttpGet("{id:guid}/{imageSize}")]
    public IActionResult Get(Guid id, ImageSize imageSize, CancellationToken cancellationToken)
    {
        var imagePath = _imagesManager.GetImagePath(id, imageSize);
        if (imagePath is null)
        {
            return NotFound();
        }
        return PhysicalFile(imagePath.Path, imagePath.ContentType);
    }

    [HttpGet("{id:guid}/metadata")]
    public IActionResult Get(Guid id)
    {
        var metadata = _imagesManager.GetMetadata(id);
        if (metadata is null)
        {
            return NotFound();
        }
        return Ok(metadata);
    }
    
    private EnhancementModel GetEnhancementModel(Enhancement enhancement)
    {
        return new EnhancementModel
        {
            Id = enhancement.Id,
            ResizedImageUrls = enhancement.ResizedImagePaths.ToDictionary(kvp => kvp.Key, kvp => GetImageUrl(kvp.Value)),
            Metadata = enhancement.Metadata,
            OriginalImageUrl = GetImageUrl(enhancement.OriginalImagePath),
            RequestId = enhancement.RequestId,
        };
    }

    private string GetImageUrl(ImagePath image)
    {
        var absolutePath = Path.GetFullPath(image.Path);
        var webRootPath = Path.GetFullPath(_webHostEnvironment.WebRootPath);
        var relativePath = Path.GetRelativePath(webRootPath, absolutePath).Replace("\\", "/");
        return $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{relativePath}";
    }
}