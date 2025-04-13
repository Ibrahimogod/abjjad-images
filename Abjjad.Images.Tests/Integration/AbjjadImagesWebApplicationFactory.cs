using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Abjjad.Images.Service;
using Abjjad.Images.Managers;
using Abjjad.Images.Core.Models;
using Abjjad.Images.Models;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Tests.Integration;

internal class AbjjadImagesWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IImageService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(s => s.GetEnhancedImage(It.IsAny<Guid>())).Returns((Enhancement?)null);
            services.AddSingleton(mockImageService.Object);

            var managerDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IImagesManager));
            if (managerDescriptor != null)
            {
                services.Remove(managerDescriptor);
            }
            
            var mockImagesManager = new Mock<IImagesManager>();
            mockImagesManager.Setup(m => m.GetMetadata(It.IsAny<Guid>())).Returns((Dictionary<string, string>?)null);
            mockImagesManager.Setup(m => m.ResizeImagesAsync(It.IsAny<IEnumerable<IFormFile>>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new ImagesUploadResponse{ ImageIds = new List<Guid>() });
            services.AddSingleton(mockImagesManager.Object);
        });
    }
}