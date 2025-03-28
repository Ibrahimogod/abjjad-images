namespace Abjjad.Images.Utils;

public interface IExifDataExtractor
{
    Task<Dictionary<string,string>> ExtractExifDataAsync(Stream imageStream, CancellationToken cancellationToken);
}