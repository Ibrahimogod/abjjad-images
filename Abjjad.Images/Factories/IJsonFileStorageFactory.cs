﻿using Abjjad.Images.Core;
using Abjjad.Images.Storage;

namespace Abjjad.Images.Factories;

public interface IJsonFileStorageFactory<TEntity, TId> 
    where TEntity : class, IEntity<TId>, new() where TId : notnull
{
    IJsonFileStorage<TEntity, TId> Create(string filePath);
}