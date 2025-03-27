using Abjjad.Images.Storage;

namespace Abjjad.Images.Factories;

public interface IImageFileStorageFactory
{
    IImageFileStorage Create(string fileStoragePath);
}