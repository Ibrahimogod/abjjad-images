using System.Text.Json.Serialization;

namespace Abjjad.Images.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<ImageSize>))]
public enum ImageSize
{
    Phone,
    Tablet,
    Desktop
}