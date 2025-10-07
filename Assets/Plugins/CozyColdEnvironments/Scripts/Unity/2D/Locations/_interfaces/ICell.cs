using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.U2D.Locations
{
    public interface ICell
    {
        Vector3Int Position { get; }
        ILocation Location { get; }
        bool HasTile { get; }

        TileBase? GetTile();
        T? GetTile<T>() where T : TileBase;

        bool TryGetTile([NotNullWhen(true)] out TileBase? tile);
        bool TryGetTile<T>([NotNullWhen(true)] out T? tile)
            where T : TileBase;

        void SetTile(TileBase? tile);

        void ClearTile();

        Bounds GetBoundsLocal();
    }
}
