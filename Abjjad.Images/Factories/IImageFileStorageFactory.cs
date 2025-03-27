using Abjjad.Images.Storage;

namespace Abjjad.Images.Factories;

public interface IImageFileStorageFactory
{
    ImageFileStorage Create(string fileStoragePath);
}