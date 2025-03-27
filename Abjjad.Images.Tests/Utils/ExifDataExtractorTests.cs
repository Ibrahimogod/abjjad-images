using Abjjad.Images.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace Abjjad.Images.Tests.Utils;

public class ExifDataExtractorTests
{
    private readonly ExifDataExtractor _exifDataExtractor;

    public ExifDataExtractorTests()
    {
        _exifDataExtractor = new ExifDataExtractor();
    }

    [Fact]
    public async Task ExtractExifDataAsync_ImageWithExifData_ExtractsAllMetadata()
    {
        // Arrange
        using var imageStream = new MemoryStream();
        using (var image = new Image<Rgba32>(100, 100))
        {
            // Add EXIF data
            var exifProfile = new ExifProfile();
            exifProfile.SetValue(ExifTag.Make, "TestMake");
            exifProfile.SetValue(ExifTag.Model, "TestModel");
            exifProfile.SetValue(ExifTag.DateTime, "2024-03-27 12:00:00");
            exifProfile.SetValue(ExifTag.DateTimeOriginal, "2024-03-27 12:00:00");
            exifProfile.SetValue(ExifTag.GPSLatitudeRef, "N");
            exifProfile.SetValue(ExifTag.GPSLatitude, new Rational[] { new Rational(40, 1), new Rational(42, 1), new Rational(0, 1) });
            exifProfile.SetValue(ExifTag.GPSLongitudeRef, "E");
            exifProfile.SetValue(ExifTag.GPSLongitude, new Rational[] { new Rational(74, 1), new Rational(0, 1), new Rational(0, 1) });

            image.Metadata.ExifProfile = exifProfile;
            image.SaveAsJpeg(imageStream);
        }
        imageStream.Position = 0;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _exifDataExtractor.ExtractExifDataAsync(imageStream, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestMake", result[nameof(ExifTag.Make)]);
        Assert.Equal("TestModel", result[nameof(ExifTag.Model)]);
        Assert.Equal("2024-03-27 12:00:00", result[nameof(ExifTag.DateTime)]);
        Assert.Equal("2024-03-27 12:00:00", result[nameof(ExifTag.DateTimeOriginal)]);
        Assert.Equal("N", result[nameof(ExifTag.GPSLatitudeRef)]);
        Assert.Equal("E", result[nameof(ExifTag.GPSLongitudeRef)]);
    }

    [Fact]
    public async Task ExtractExifDataAsync_ImageWithoutExifData_ReturnsEmptyDictionary()
    {
        // Arrange
        using var imageStream = new MemoryStream();
        using (var image = new Image<Rgba32>(100, 100))
        {
            image.SaveAsJpeg(imageStream);
        }
        imageStream.Position = 0;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _exifDataExtractor.ExtractExifDataAsync(imageStream, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExtractExifDataAsync_InvalidImageStream_ReturnsEmptyDictionary()
    {
        // Arrange
        using var imageStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _exifDataExtractor.ExtractExifDataAsync(imageStream, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
} 