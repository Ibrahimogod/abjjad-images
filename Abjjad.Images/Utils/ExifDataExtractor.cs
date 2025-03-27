using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace Abjjad.Images.Utils;

public class ExifDataExtractor
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

            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.Make, out var make))
            {
                metadata.Add(nameof(ExifTag.Make), make.Value);
            }
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.Model, out var model))
            {
                metadata.Add(nameof(ExifTag.Model), model.Value);
            }
            
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.DateTime, out var dateTime))
            {
                metadata.Add(nameof(ExifTag.DateTime), dateTime.Value);
            }
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.DateTimeOriginal, out var dateTimeOriginal))
            {
                metadata.Add(nameof(ExifTag.DateTimeOriginal), dateTimeOriginal.Value);
            }
            
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLatitudeRef, out var latitudeRef))
            {
                metadata.Add(nameof(ExifTag.GPSLatitudeRef), latitudeRef.Value);
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLatitude, out var latitude))
            {
                metadata.Add(nameof(ExifTag.GPSLatitude), latitude.Value.ToString());
            }
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSDestLatitudeRef, out var destLatitudeRef))
            {
                metadata.Add(nameof(ExifTag.GPSDestLatitudeRef), destLatitudeRef.Value);
            }
            
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLongitudeRef, out var longitudeRef))
            {
                metadata.Add(nameof(ExifTag.GPSLongitudeRef), longitudeRef.Value);
            }
            if (image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSLongitude, out var longitude))
            {
                metadata.Add(nameof(ExifTag.GPSLongitude), longitude.Value.ToString());
            }
            if(image.Metadata.ExifProfile.TryGetValue(ExifTag.GPSDestLongitudeRef, out var destLongitudeRef))
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

    // public ImageMetadata ExtractExifData(Stream imageStream)
    // {
    //     try
    //     {
    //         imageStream.Position = 0;
    //         using var image = Image.Load(imageStream);
    //         
    //         if (image.Metadata.ExifProfile == null)
    //         {
    //             return new ImageMetadata();
    //         }
    //
    //         return new ImageMetadata
    //         {
    //             Make = GetExifValue(image.Metadata.ExifProfile, ExifTag.Make),
    //             Model = GetExifValue(image.Metadata.ExifProfile, ExifTag.Model),
    //             Latitude = GetExifValue(image.Metadata.ExifProfile, ExifTag.GPSLatitudeRef),
    //             Longitude = GetExifValue(image.Metadata.ExifProfile, ExifTag.GPSLongitudeRef),
    //         };
    //     }
    //     catch
    //     {
    //         return new ImageMetadata();
    //     }
    // }

    private T GetExifValue<T>(ExifProfile profile, ExifTag<T> tag)
    {
        if (profile.TryGetValue(tag, out var value) ) //&& value.Value != default
        {
            return value.Value;
        }
        return default;
    }
}