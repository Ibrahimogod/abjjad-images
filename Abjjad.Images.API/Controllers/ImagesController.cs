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
        var validationTasks = model.Images.Select(async image =>
        {
            if (image.Length > MAX_FILE_SIEZE)
            {
                return (false, $"File '{image.FileName}' size is too large");
            }

            if (!ALLOWED_CONTENT_TYPES.Contains(image.ContentType))
            {
                return (false, $"File '{image.FileName}' type is not supported");
            }

            if (!ALLOWED_EXTENSIONS.Contains(Path.GetExtension(image.FileName).ToLower()))
            {
                return (false, $"File '{image.FileName}' extension is not supported");
            }

            return (true, string.Empty);
        });

        var validationResults = await Task.WhenAll(validationTasks);
        var invalidFiles = validationResults.Where(r => !r.Item1).ToList();
        
        if (invalidFiles.Any())
        {
            foreach (var (_, error) in invalidFiles)
            {
                ModelState.AddModelError("Images", error);
            }
            return BadRequest(ModelState);
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