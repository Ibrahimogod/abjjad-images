using Moq;
using Abjjad.Images.Managers;
using Abjjad.Images.Service;
using Abjjad.Images.Core.Enums;
using Abjjad.Images.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Tests.Managers;

public class ImagesManagerTests
{
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly ImagesManager _imagesManager;

    public ImagesManagerTests()
    {
        _imageServiceMock = new Mock<IImageService>();
        _imagesManager = new ImagesManager(_imageServiceMock.Object);
    }

    [Fact]
    public async Task ResizeImagesAsync_MultipleFiles_ReturnsAllImageIds()
    {
        // Arrange
        var files = new[]
        {
            new Mock<IFormFile>().Object,
            new Mock<IFormFile>().Object,
            new Mock<IFormFile>().Object
        };
        var requestId = "test-request-id";
        var cancellationToken = CancellationToken.None;
        var expectedIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        _imageServiceMock.Setup(x => x.ResizeImageAsync(
            It.IsAny<IFormFile>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((IFormFile f, string r, CancellationToken c) => 
            expectedIds[Array.IndexOf(files, f)]);

        // Act
        var result = await _imagesManager.ResizeImagesAsync(files, requestId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.ImageIds.Count());
        Assert.Equal(expectedIds, result.ImageIds);
        _imageServiceMock.Verify(x => x.ResizeImageAsync(
            It.IsAny<IFormFile>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(3));
    }

    [Fact]
    public void GetImagePath_ValidIdAndSize_ReturnsImagePath()
    {
        // Arrange
        var id = Guid.NewGuid();
        var size = ImageSize.Phone;
        var expectedPath = new ImagePath { Path = "test/path", ContentType = "image/webp" };
        var enhancement = new Enhancement
        {
            Id = id,
            ResizedImagePaths = new Dictionary<ImageSize, ImagePath>
            {
                { size, expectedPath }
            }
        };

        _imageServiceMock.Setup(x => x.GetEnhancedImage(id))
            .Returns(enhancement);

        // Act
        var result = _imagesManager.GetImagePath(id, size);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPath.Path, result.Path);
        Assert.Equal(expectedPath.ContentType, result.ContentType);
    }

    [Fact]
    public void GetImagePath_InvalidId_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var size = ImageSize.Phone;

        _imageServiceMock.Setup(x => x.GetEnhancedImage(id))
            .Returns((Enhancement?)null);

        // Act
        var result = _imagesManager.GetImagePath(id, size);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMetadata_ValidId_ReturnsMetadata()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedMetadata = new Dictionary<string, string>
        {
            { "Make", "TestMake" },
            { "Model", "TestModel" }
        };
        var enhancement = new Enhancement
        {
            Id = id,
            Metadata = expectedMetadata
        };

        _imageServiceMock.Setup(x => x.GetEnhancedImage(id))
            .Returns(enhancement);

        // Act
        var result = _imagesManager.GetMetadata(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMetadata, result);
    }

    [Fact]
    public void GetMetadata_InvalidId_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _imageServiceMock.Setup(x => x.GetEnhancedImage(id))
            .Returns((Enhancement?)null);

        // Act
        var result = _imagesManager.GetMetadata(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllEnhancements_ReturnsAllEnhancements()
    {
        // Arrange
        var expectedEnhancements = new List<Enhancement>
        {
            new Enhancement { Id = Guid.NewGuid() },
            new Enhancement { Id = Guid.NewGuid() }
        };

        _imageServiceMock.Setup(x => x.GetAllEnhancements())
            .Returns(expectedEnhancements);

        // Act
        var result = _imagesManager.GetAllEnhancements();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(expectedEnhancements, result);
    }
} 