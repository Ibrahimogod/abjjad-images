namespace Abjjad.Images.Storage;

public class ImageFileStorage : IImageFileStorage
{
    private readonly string _storagePath;
    
    public ImageFileStorage(string storagePath)
    {
        _storagePath = storagePath;
    }
    
    public async Task<string> SaveAsync(string directoryPath, string fileName, Stream stream, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_storagePath, directoryPath, fileName);
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await using var fileStream = File.Create(filePath);
        stream.Position = 0;
        await stream.CopyToAsync(fileStream, cancellationToken);
        return filePath;
    }
}