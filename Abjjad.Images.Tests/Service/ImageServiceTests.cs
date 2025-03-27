using Moq;
using Abjjad.Images.Utils;
using Abjjad.Images.Models;
using Abjjad.Images.Service;
using Abjjad.Images.Storage;
using Abjjad.Images.Core.Models;
using Abjjad.Images.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Abjjad.Images.Tests.Service;

public class ImageServiceTests
{
    private readonly Mock<IImageFileStorage> _imageFileStorageMock;
    private readonly Mock<IExifDataExtractor> _exifDataExtractorMock;
    private readonly Mock<IJsonFileStorage<Enhancement, Guid>> _enhancementFileStorageMock;
    private readonly Mock<IImageProcessor> _imageProcessorMock;
    private readonly ImageService _imageService;

    public ImageServiceTests()
    {
        _imageFileStorageMock = new Mock<IImageFileStorage>();
        _exifDataExtractorMock = new Mock<IExifDataExtractor>();
        _enhancementFileStorageMock = new Mock<IJsonFileStorage<Enhancement, Guid>>();
        _imageProcessorMock = new Mock<IImageProcessor>();

        _imageService = new ImageService(
            _imageFileStorageMock.Object,
            _exifDataExtractorMock.Object,
            _enhancementFileStorageMock.Object,
            _imageProcessorMock.Object
        );
    }

    [Fact]
    public async Task ResizeImageAsync_ValidFile_ReturnsEnhancementId()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var requestId = "test-request-id";
        var cancellationToken = CancellationToken.None;
        var enhancementId = Guid.NewGuid();

        // Setup mocks
        _imageFileStorageMock.Setup(x => x.SaveAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync("test-path");

        _exifDataExtractorMock.Setup(x => x.ExtractExifDataAsync(
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(new Dictionary<string, string>());

        _imageProcessorMock.Setup(x => x.ProcessImageAsync(
            It.IsAny<Stream>(),
            It.IsAny<ImageSize>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(new ProcessedImageResult { Extension = ".jpg", ContentType = "image/jpeg", Stream = new MemoryStream() });

        // Act
        var result = await _imageService.ResizeImageAsync(file.Object, requestId, cancellationToken);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _imageFileStorageMock.Verify(x => x.SaveAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(4)); // Original + 3 resized versions
    }

    [Fact]
    public void GetEnhancedImage_ValidId_ReturnsEnhancement()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedEnhancement = new Enhancement { Id = id };

        _enhancementFileStorageMock.Setup(x => x.GetById(id))
            .Returns(expectedEnhancement);

        // Act
        var result = _imageService.GetEnhancedImage(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
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

        _enhancementFileStorageMock.Setup(x => x.GetAll())
            .Returns(expectedEnhancements);

        // Act
        var result = _imageService.GetAllEnhancements();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
} 