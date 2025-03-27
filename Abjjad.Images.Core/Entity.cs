namespace Abjjad.Images.Core;

public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
}