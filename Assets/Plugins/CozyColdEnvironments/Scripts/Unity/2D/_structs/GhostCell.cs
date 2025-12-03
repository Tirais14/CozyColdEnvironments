using CCEnvs.FuncLanguage;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public struct GhostCell : IDisposable
    {
        private readonly TileBase tile;
        private readonly Tilemap tilemap;
        private readonly Maybe<GameObject> linkedGO;
        private readonly Tile ghostTile;

        public GhostCell(
            TileBase tile,
            Sprite? tileSprite,
            GameObject? tilePrefab,
            Tilemap tilemap)
            :
            this()
        {
            CC.Guard.IsNotNull(tilemap, nameof(tilemap));  
            CC.Guard.IsNotNull(tile, nameof(tile));

            this.tile = tile;
            this.tilemap = tilemap;

            ghostTile = ScriptableObject.CreateInstance<Tile>();
            ghostTile.sprite = tileSprite;

            if (tilePrefab != null)
            {
                linkedGO = Object.Instantiate(
                    tilePrefab,
                    ghostTile.transform.GetPosition(),
                    Quaternion.identity,
                    tilemap.transform
                    );
            }
        }

        public GhostCell(ICell cell, Tilemap tilemap)
            :
            this(cell.GetTile().Raw!, cell.GetTileSprite().Raw, cell.GetTilePrefab().Raw, tilemap)
        {
        }

        public readonly void SetPosition(Vector3Int pos)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Matrix4x4 tempTileTransform = ghostTile.transform;
            tempTileTransform.MultiplyPoint3x4(pos);

            if (linkedGO.TryGetValue(out GameObject? go))
                go.transform.position = pos;

            tilemap.SetTile(pos, ghostTile);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            linkedGO.IfSome(go => Object.Destroy(go));
            Object.Destroy(ghostTile);
            disposed = true;
        }
    }
}
