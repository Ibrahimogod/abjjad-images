using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Abjjad.Images.Options;

namespace Abjjad.Images.API.HealthChecks;

public class DataFileHealthCheck : IHealthCheck
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly StorageOptions _storageOptions;

    public DataFileHealthCheck(IWebHostEnvironment webHostEnvironment, IOptions<StorageOptions> storageOptions)
    {
        _webHostEnvironment = webHostEnvironment;
        _storageOptions = storageOptions.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var dataFilePath = Path.Combine(_webHostEnvironment.WebRootPath, _storageOptions.BaseFilesPath);
        
        try
        {
            await using (_ = new FileStream(dataFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                return HealthCheckResult.Healthy("Data file is accessible for reading and writing");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Data file is not accessible", ex);
        }
    }
} 