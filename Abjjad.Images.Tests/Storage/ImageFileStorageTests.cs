using Abjjad.Images.Storage;
using System.Text;

namespace Abjjad.Images.Tests.Storage;

public class ImageFileStorageTests : IDisposable
{
    private readonly string _testStoragePath;
    private readonly ImageFileStorage _imageFileStorage;

    public ImageFileStorageTests()
    {
        _testStoragePath = Path.Combine(Path.GetTempPath(), "AbjjadImagesTest_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testStoragePath);
        _imageFileStorage = new ImageFileStorage(_testStoragePath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testStoragePath))
        {
            Directory.Delete(_testStoragePath, true);
        }
    }

    [Fact]
    public async Task SaveAsync_ValidFile_SavesFileCorrectly()
    {
        // Arrange
        var directoryPath = "test-directory";
        var fileName = "test-image.jpg";
        var content = "Test image content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _imageFileStorage.SaveAsync(directoryPath, fileName, stream, cancellationToken);

        // Assert
        Assert.True(File.Exists(result));
        Assert.Equal(Path.Combine(_testStoragePath, directoryPath, fileName), result);
        
        var savedContent = await File.ReadAllTextAsync(result);
        Assert.Equal(content, savedContent);
    }

    [Fact]
    public async Task SaveAsync_NestedDirectory_CreatesDirectoryStructure()
    {
        // Arrange
        var directoryPath = "test/directory/structure";
        var fileName = "test-image.jpg";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Test content"));
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _imageFileStorage.SaveAsync(directoryPath, fileName, stream, cancellationToken);

        // Assert
        Assert.True(File.Exists(result));
        Assert.Equal(Path.Combine(_testStoragePath, directoryPath, fileName), result);
    }

    [Fact]
    public async Task SaveAsync_StreamAtEnd_PositionResetToStart()
    {
        // Arrange
        var directoryPath = "test-directory";
        var fileName = "test-image.jpg";
        var content = "Test image content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        stream.Position = stream.Length; // Move to end of stream
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _imageFileStorage.SaveAsync(directoryPath, fileName, stream, cancellationToken);

        // Assert
        var savedContent = await File.ReadAllTextAsync(result);
        Assert.Equal(content, savedContent);
    }
} 