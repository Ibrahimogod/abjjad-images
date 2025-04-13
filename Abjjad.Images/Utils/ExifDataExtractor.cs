using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace Abjjad.Images.Utils;

public class ExifDataExtractor : IExifDataExtractor
{
    
    public async Task<Dictionary<string,string>> ExtractExifDataAsync(Stream imageStream, CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, string>();
        try
        {
            imageStream.Position = 0;
            using var image = await Image.LoadAsync(imageStream, cancellationToken);
            if (image.Metadata.ExifProfile == null)
            {
                return metadata;
            }

            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.Make, out var make) && make?.Value != null)
            {
                metadata.Add(nameof(ExifTag.Make), make.Value);
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.Model, out var model) && model?.Value != null)
            {
                metadata.Add(nameof(ExifTag.Model), model.Value);
            }
            
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.DateTime, out var dateTime) && dateTime?.Value != null)
            {
                metadata.Add(nameof(ExifTag.DateTime), dateTime.Value);
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.DateTimeOriginal, out var dateTimeOriginal) && dateTimeOriginal?.Value != null)
            {
                metadata.Add(nameof(ExifTag.DateTimeOriginal), dateTimeOriginal.Value);
            }
            
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLatitudeRef, out var latitudeRef) && latitudeRef?.Value != null)
            {
                metadata.Add(nameof(ExifTag.GPSLatitudeRef), latitudeRef.Value);
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLatitude, out var latitude) && latitude?.Value != null)
            {
                metadata.Add(nameof(ExifTag.GPSLatitude), latitude.Value.ToString());
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSDestLatitudeRef, out var destLatitudeRef) && destLatitudeRef?.Value != null)
            {
                metadata.Add(nameof(ExifTag.GPSDestLatitudeRef), destLatitudeRef.Value);
            }
            
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLongitudeRef, out var longitudeRef) && longitudeRef?.Value != null)
            {
                metadata.Add(nameof(ExifTag.GPSLongitudeRef), longitudeRef.Value);
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLongitude, out var longitude) && longitude?.Value != null)
            {
                metadata.Add(nameof(ExifTag.GPSLongitude), longitude.Value.ToString());
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSDestLongitudeRef, out var destLongitudeRef) && destLongitudeRef?.Value != null)
            {
                metadata.Add(nameof(ExifTag.GPSDestLongitudeRef), destLongitudeRef.Value);
            }
            
            return metadata;
        }
        catch
        {
            return metadata;
        }
    }
}