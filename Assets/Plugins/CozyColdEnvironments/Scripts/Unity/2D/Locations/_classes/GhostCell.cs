using CCEnvs.FuncLanguage;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class GhostCell : IGhostCell
    {
        private readonly Maybe<TileBase> tile;
        private readonly Tilemap tilemap;
        private readonly Maybe<GameObject> linkedGO;
        private readonly Maybe<Tile> ghostTile;
        private Maybe<Vector3Int> ghostTilePos;
        private ReactiveCommand<Unit>? materilaizeCommand;

        public GhostCell(
            TileBase? tile,
            Sprite? tileSprite,
            GameObject? tilePrefab,
            Tilemap tilemap)
        {
            CC.Guard.IsNotNull(tilemap, nameof(tilemap));  

            this.tile = tile;
            this.tilemap = tilemap;

            var ghostTile = ScriptableObject.CreateInstance<Tile>();
            ghostTile.name = "GhostTile";
            this.ghostTile = ghostTile;

            if (tile == null)
                return;

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
            this(cell.GetTile().Raw, cell.GetTileSprite().Raw, cell.GetTilePrefab().Raw, tilemap)
        {
        }

        public void SetPosition(Vector3Int pos)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!this.ghostTile.TryGetValue(out Tile? ghostTile))
                return;

            if (ghostTilePos.Has(pos))
                return;

            if (ghostTilePos.TryGetValue(out Vector3Int previousPos))
                tilemap.SetTile(previousPos, tile: null);

            if (linkedGO.TryGetValue(out GameObject? go))
                go.transform.position = tilemap.GetCellCenterWorld(pos);

            tilemap.SetTile(pos, ghostTile);
            ghostTilePos = pos;
        }

        public void Materialize(Tilemap? otherTilemap = null)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (otherTilemap == null)
                otherTilemap = tilemap;

            if (ghostTilePos.TryGetValue(out Vector3Int pos))
                otherTilemap.SetTile(pos, tile.Raw);

            materilaizeCommand?.Execute(Unit.Default);
        }

        public IObservable<Unit> ObserveMaterialize()
        {
            materilaizeCommand ??= new ReactiveCommand<Unit>();
            return materilaizeCommand;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            linkedGO.IfSome(go => Object.Destroy(go));
            ghostTile.IfSome(tile => Object.Destroy(tile));
            materilaizeCommand?.Dispose();
            disposed = true;
        }
    }
}
