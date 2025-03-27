using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace Abjjad.Images.Utils;

public interface IExifDataExtractor
{
    Task<Dictionary<string,string>> ExtractExifDataAsync(Stream imageStream, CancellationToken cancellationToken);
    T GetExifValue<T>(ExifProfile profile, ExifTag<T> tag);
}