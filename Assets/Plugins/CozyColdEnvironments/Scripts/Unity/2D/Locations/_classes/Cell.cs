using CCEnvs.FuncLanguage;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class Cell : ICell
    {
        public ILocationLayer Location { get; }
        public Vector3Int Position { get; }
        public Maybe<object> Owner { get; private set; }

        public Cell(ILocationLayer location, Vector3Int position)
        {
            CC.Guard.IsNotNull(location, nameof(location));

            Location = location;
            Position = position;
        }

        public Cell(ILocationLayer location, Vector3Int position, object? owner)
            :
            this(location, position)
        {
            Owner = owner;
        }

        public Maybe<TileBase> GetTile()
        {
            return Location.tilemap.GetTile(Position);
        }
        public Maybe<T> GetTile<T>() where T : TileBase
        {
            return Location.tilemap.GetTile<T>(Position);
        }

        public void SetTile(TileBase? tile)
        {
            Location.tilemap.SetTile(Position, tile);
        }

        public bool RemoveTile([NotNullWhen(true)] out TileBase? tile)
        {
            if (GetTile().TryGetValue(out tile))
            {
                SetTile(null);
                return true;
            }

            return false;
        }
        public bool RemoveTile() => RemoveTile(out _);

        public Bounds GetBounds() => Location.tilemap.GetBoundsLocal(Position);

        public bool HasTile() => GetTile().IsSome;

        public bool HasOwner() => Owner.IsSome;
        public bool HasOwner(object owner) => Owner.Has(owner);

        public bool SetOwner(object owner)
        {
            Owner = owner;
            return true;
        }
    }
}
