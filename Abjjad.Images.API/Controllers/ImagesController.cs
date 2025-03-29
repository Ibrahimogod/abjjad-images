using Abjjad.Images.Managers;
using Microsoft.AspNetCore.Mvc;
using Abjjad.Images.API.Models;
using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;
using Abjjad.Images.Models;

namespace Abjjad.Images.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private const string REQUIRED_CONTENT_TYPE = "multipart/form-data";
    private const int MAX_FILE_SIZE = 2 * 1024 * 1024; // 2MB
    private readonly string[] ALLOWED_CONTENT_TYPES = { "image/jpg", "image/jpeg", "image/png", "image/webp" };
    private readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };
    
    private readonly ImagesManager _imagesManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        ImagesManager imagesManager, 
        IWebHostEnvironment webHostEnvironment,
        ILogger<ImagesController> logger)
    {
        _imagesManager = imagesManager;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }
    
    [HttpPost]
    [Consumes(REQUIRED_CONTENT_TYPE)]
    [ProducesResponseType(typeof(ImagesUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upload(UploadImagesModel model, CancellationToken cancellationToken)
    {
        try
        {
            if (model?.Images == null || !model.Images.Any())
            {
                ModelState.AddModelError("Images", "No images were provided");
                return BadRequest(ModelState);
            }

            var validationTasks = model.Images.Select(async image =>
            {
                if (image.Length > MAX_FILE_SIZE)
                {
                    return (false, $"File '{image.FileName}' size is too large. Maximum size is 2MB");
                }

                if (!ALLOWED_CONTENT_TYPES.Contains(image.ContentType))
                {
                    return (false, $"File '{image.FileName}' type is not supported. Supported types are: {string.Join(", ", ALLOWED_CONTENT_TYPES)}");
                }

                if (!ALLOWED_EXTENSIONS.Contains(Path.GetExtension(image.FileName).ToLower()))
                {
                    return (false, $"File '{image.FileName}' extension is not supported. Supported extensions are: {string.Join(", ", ALLOWED_EXTENSIONS)}");
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image upload");
            return StatusCode(500, "An error occurred while processing the images");
        }
    }

    [HttpGet]
    public IActionResult Get()
    {
        var enhancements = _imagesManager.GetAllEnhancements();
        var models = enhancements?.Select(GetEnhancementModel);
        return Ok(models);
    }

    [HttpGet("{id:guid}/{imageSize}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Get(Guid id, ImageSize imageSize, CancellationToken cancellationToken)
    {
        try
        {
            var imagePath = _imagesManager.GetImagePath(id, imageSize);
            if (imagePath is null)
            {
                ModelState.AddModelError("Image", $"Image with ID {id} and size {imageSize} not found");
                return NotFound(ModelState);
            }
            return PhysicalFile(imagePath.Path, imagePath.ContentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image {ImageId} with size {ImageSize}", id, imageSize);
            return StatusCode(500, "An error occurred while retrieving the image");
        }
    }

    [HttpGet("{id:guid}/metadata")]
    [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetMetadata(Guid id)
    {
        try
        {
            var metadata = _imagesManager.GetMetadata(id);
            if (metadata is null)
            {
                ModelState.AddModelError("Metadata", $"Metadata for image with ID {id} not found");
                return NotFound(ModelState);
            }
            return Ok(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata for image {ImageId}", id);
            return StatusCode(500, "An error occurred while retrieving the metadata");
        }
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