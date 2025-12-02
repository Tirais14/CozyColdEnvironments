using CCEnvs.FuncLanguage;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class Cell : ICell
    {
        private Maybe<TileBase> tile = null!;

        public ILocationLayer LocationLayer { get; }
        public Vector3Int Position { get; }
        public Maybe<object> Owner { get; private set; }

        public Cell(
            ILocationLayer locationLayer,
            Vector3Int position,
            TileBase? tile = null,
            object? owner = null)
        {
            CC.Guard.IsNotNull(locationLayer, nameof(locationLayer));

            LocationLayer = locationLayer;
            Position = position;
            this.tile = tile;
            Owner = owner;

            Refresh();
        }

        public Cell(
            ILocationLayer locationLayer,
            Vector2Int position,
            TileBase? tile = null,
            object? owner = null)
            :
            this(locationLayer, (Vector3Int)position, tile: tile, owner: owner)
        {
        }
        public Cell(
            ILocationLayer locationLayer,
            Vector3 position,
            TileBase? tile = null,
            object? owner = null)
            :
            this(locationLayer, locationLayer.ConvertPosition(position), tile: tile, owner: owner)
        {
        }

        public Cell(
            ILocationLayer locationLayer,
            Vector2 position,
            TileBase? tile = null,
            object? owner = null)
            :
            this(locationLayer, locationLayer.ConvertPosition(position), tile: tile, owner: owner)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TileBase> GetTile() => tile;

        public Maybe<T> GetTile<T>() where T : TileBase
        {
            return GetTile().Raw.As<T>();
        }

        public void SetTile(TileBase? tile)
        {
            this.tile = tile;
            LocationLayer.tilemap.SetTile(Position, tile);
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

        public Bounds GetBounds() => LocationLayer.tilemap.GetBoundsLocal(Position);

        public bool HasTile() => GetTile().IsSome;

        public bool HasOwner() => Owner.IsSome;
        public bool HasOwner(object owner) => Owner.Has(owner);

        public bool SetOwner(object owner)
        {
            Owner = owner;
            return true;
        }
        public void Refresh()
        {
            if (tile.Raw != LocationLayer.tilemap.GetTile(Position))
                LocationLayer.tilemap.SetTile(Position, tile.Raw);
        }

        public override string ToString()
        {
            return $"{nameof(tile)}: {tile}; {nameof(Position)}: {Position}; {nameof(LocationLayer)}: {LocationLayer}; {nameof(Owner)}: {Owner}";
        }
    }
}
