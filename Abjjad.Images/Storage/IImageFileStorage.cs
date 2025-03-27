namespace Abjjad.Images.Storage;

public interface IImageFileStorage
{
    Task<string> SaveAsync(string directoryPath, string fileName, Stream stream, CancellationToken cancellationToken);
}