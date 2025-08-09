using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace UTIRLib.TwoD
{
    public class LocationTileInfo : IReadOnlyTileProvider
    {
        protected readonly Vector2Int position;
        protected readonly TileBase? tile;
        protected readonly bool hasTile;

        public Vector2Int Position => position;
        public TileBase? Tile => tile;
        public bool HasTile => hasTile;

        public LocationTileInfo(Vector2Int position, TileBase? tile = null)
        {
            this.position = position;
            this.tile = tile;
            hasTile = tile != null;
        }

        public LocationTileInfo(TileBase? tile = null) : this(Vector2Int.zero, tile)
        {
        }

        public T? GetTile<T>() where T : TileBase => tile as T;

        public bool TryGetTile<T>([NotNullWhen(true)] out T? tile) where T : TileBase
        {
            tile = GetTile<T>();

            return tile != null;
        }

        public override int GetHashCode() => HashCode.Combine(position, tile, hasTile);

        public override bool Equals(object obj) => obj != null && GetHashCode() == obj.GetHashCode() ||
            ReferenceEquals(this, obj);
    }

    public class LocationTileInfo<T> : LocationTileInfo, IReadOnlyTileProvider<T>
        where T : TileBase
    {
        public new T? Tile => GetTile<T>();
    }
}