using CCEnvs.FuncLanguage;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public readonly struct MaterializedCellInfo
    {
        public Tilemap tilemap { get; }
        public Maybe<TileBase> Tile { get; }
        public Maybe<GameObject> InstantiatedTilePrefab { get; }
        public Vector3Int Position { get; }

        public MaterializedCellInfo(Tilemap tilemap, TileBase? tile, Vector3Int position)
        {
            this.tilemap = tilemap;
            Tile = tile;
            Position = position;

            InstantiatedTilePrefab = tilemap.GetInstantiatedObject(position);
        }
    }
}
