using Abjjad.Images.Utils;
using Abjjad.Images.Options;
using Abjjad.Images.Core.Enums;
using Microsoft.Extensions.Options;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Abjjad.Images.Tests.Utils;

public class ImageProcessorTests
{
    private readonly ImageProcessingOptions _options;
    private readonly ImageProcessor _imageProcessor;

    public ImageProcessorTests()
    {
        _options = new ImageProcessingOptions
        {
            PhoneWidth = 800,
            PhoneHeight = 600,
            TabletWidth = 1024,
            TabletHeight = 768,
            DesktopWidth = 1920,
            DesktopHeight = 1080,
            Quality = 80,
            Method = 4
        };

        var optionsMock = new Mock<IOptions<ImageProcessingOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        _imageProcessor = new ImageProcessor(optionsMock.Object);
    }

    [Fact]
    public async Task ProcessImageAsync_ValidImage_ReturnsProcessedImage()
    {
        // Arrange
        using var imageStream = new MemoryStream();
        using (var image = new Image<Rgba32>(100, 100))
        {
            image.SaveAsWebp(imageStream);
        }
        imageStream.Position = 0;
        var size = ImageSize.Phone;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _imageProcessor.ProcessImageAsync(imageStream, size, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(".webp", result.Extension);
        Assert.Equal("image/webp", result.ContentType);
        Assert.NotNull(result.Stream);
    }

    [Theory]
    [InlineData(ImageSize.Phone, 800, 600)]
    [InlineData(ImageSize.Tablet, 1024, 768)]
    [InlineData(ImageSize.Desktop, 1920, 1080)]
    public void GetDimensionsForSize_ValidSize_ReturnsCorrectDimensions(ImageSize size, int expectedWidth, int expectedHeight)
    {
        // Act
        var dimensions = _imageProcessor.GetDimensionsForSize(size);

        // Assert
        Assert.Equal(expectedWidth, dimensions.Width);
        Assert.Equal(expectedHeight, dimensions.Height);
    }

    [Fact]
    public void GetDimensionsForSize_InvalidSize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidSize = (ImageSize)999;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _imageProcessor.GetDimensionsForSize(invalidSize));
    }
} 