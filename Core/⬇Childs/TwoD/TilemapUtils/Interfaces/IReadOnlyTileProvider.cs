using System.Diagnostics.CodeAnalysis;
using UnityEngine.Tilemaps;

#nullable enable

namespace UTIRLib.TwoD
{
    public interface IReadOnlyTileProvider
    {
        TileBase? Tile { get; }
        bool HasTile { get; }

        T? GetTile<T>() where T : TileBase;

        bool TryGetTile<T>([NotNullWhen(true)] out T? tile) where T : TileBase;
    }

    public interface IReadOnlyTileProvider<out T> : IReadOnlyTileProvider
        where T : TileBase
    {
        new T? Tile { get; }
    }
}