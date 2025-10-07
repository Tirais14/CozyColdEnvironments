using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.U2D.Locations
{
    public readonly struct Cell : ICell, IEquatable<Cell>
    {
        public ILocation Location { get; }
        public Vector3Int Position { get; }
        public bool HasTile => GetTile() != null;

        public Cell(ILocation location, Vector3Int position)
        {
            Location = location;
            Position = position;
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !(left == right);
        }

        public TileBase? GetTile()
        {
            return Location.Map.GetTile(Position);
        }
        public T? GetTile<T>() where T : TileBase
        {
            return Location.Map.GetTile<T>(Position);
        }

        public bool TryGetTile([NotNullWhen(true)] out TileBase? tile)
        {
            tile = GetTile();

            return tile != null;
        }

        public bool TryGetTile<T>([NotNullWhen(true)] out T? tile) where T : TileBase
        {
            tile = GetTile<T>();

            return tile != null;
        }

        public void SetTile(TileBase? tile)
        {
            Location.Map.SetTile(Position, tile);
        }

        public void ClearTile() => SetTile(null);

        public Bounds GetBoundsLocal() => Location.Map.GetBoundsLocal(Position);

        public bool Equals(Cell other)
        {
            return Location.Equals(other.Location)
                   &&
                   Position == other.Position;
        }
        public override bool Equals(object obj)
        {
            return obj is Cell typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Location, Position);
        }
    }
}
