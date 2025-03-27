namespace Abjjad.Images.Core;

public interface IEntity<TId>
{
    TId Id { get; set; }
}