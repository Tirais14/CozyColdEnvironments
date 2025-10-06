using UnityEngine.Tilemaps;

#nullable enable

namespace CCEnvs.Unity.TwoD.Tilemaps
{
    public interface ITileProvider : IReadOnlyTileProvider
    {
        new TileBase? Tile { get; set; }

        void SetTile(TileBase? tile);
    }

    public interface ITileProvider<T> : IReadOnlyTileProvider<T>, ITileProvider
        where T : TileBase
    {
        new T? Tile { get; set; }

        void SetTile(T? tile);
    }
}