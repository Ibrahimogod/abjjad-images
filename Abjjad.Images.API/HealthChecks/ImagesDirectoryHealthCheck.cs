using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Abjjad.Images.Options;

namespace Abjjad.Images.API.HealthChecks;

public class ImagesDirectoryHealthCheck : IHealthCheck
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly StorageOptions _storageOptions;

    public ImagesDirectoryHealthCheck(IWebHostEnvironment webHostEnvironment, IOptions<StorageOptions> storageOptions)
    {
        _webHostEnvironment = webHostEnvironment;
        _storageOptions = storageOptions.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, _storageOptions.BaseImagesPath);
        
        if (!Directory.Exists(imagesPath))
        {
            try
            {
                Directory.CreateDirectory(imagesPath);
                return HealthCheckResult.Healthy("Images directory created successfully");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Failed to create images directory", ex);
            }
        }
        
        try
        {
            var testFile = Path.Combine(imagesPath, ".healthcheck");
            await File.WriteAllTextAsync(testFile, DateTime.UtcNow.ToString(), cancellationToken);
            File.Delete(testFile);
            return HealthCheckResult.Healthy("Images directory is accessible and writable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Images directory is not writable", ex);
        }
    }
} 