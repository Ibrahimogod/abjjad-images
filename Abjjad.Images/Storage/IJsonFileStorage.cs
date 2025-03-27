using Abjjad.Images.Core;
using Microsoft.Extensions.Hosting;

namespace Abjjad.Images.Storage;

public interface IJsonFileStorage<TEntity, TId> : IHostedService
    where TEntity : class, IEntity<TId>, new() 
    where TId : notnull
{
    IEnumerable<TEntity> GetAll();
    TEntity? GetById(TId id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TId id);
}