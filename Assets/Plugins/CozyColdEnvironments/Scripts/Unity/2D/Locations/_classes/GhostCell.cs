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
        private readonly Maybe<GameObject> linkedGO;
        private readonly Maybe<Tile> ghostTile;
        private ReactiveCommand<Unit>? materilaizeCommand;

        public Maybe<TileBase> Tile { get; }
        public Tilemap tilemap { get; }
        public Maybe<Vector3Int> Position { get; private set; }

        public GhostCell(
            TileBase? tile,
            Sprite? tileSprite,
            GameObject? tilePrefab,
            Tilemap tilemap)
        {
            CC.Guard.IsNotNull(tilemap, nameof(tilemap));  

            Tile = tile;
            this.tilemap = tilemap;

            if (tile == null)
                return;

            if (tileSprite == null)
                tileSprite = tile.GetTileSprite().Raw;

            var ghostTile = ScriptableObject.CreateInstance<Tile>();
            ghostTile.name = $"GhostTile({tile.name})";
            ghostTile.sprite = tileSprite;
            ghostTile.hideFlags = tile.hideFlags;

            if (tilePrefab == null)
                tilePrefab = tile.GetTileGameObject().Raw;

            if (tilePrefab != null)
            {
                linkedGO = Object.Instantiate(
                    tilePrefab,
                    ghostTile.transform.GetPosition(),
                    Quaternion.identity,
                    tilemap.transform
                    );
            }

            this.ghostTile = ghostTile;
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

            if (Position.Has(pos))
                return;

            if (Position.TryGetValue(out Vector3Int previousPos))
                tilemap.SetTile(previousPos, tile: null);

            if (linkedGO.TryGetValue(out GameObject? go))
                go.transform.position = tilemap.GetCellCenterWorld(pos);

            tilemap.SetTile(pos, ghostTile);
            Position = pos;
        }

        public void Materialize(Tilemap? otherTilemap = null)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (Position.TryGetValue(out Vector3Int pos))
            {
                if (otherTilemap != null)
                {
                    if (tilemap.transform != otherTilemap.transform)
                        this.PrintWarning("Tilemap transforms not equals");
                    if (tilemap.layoutGrid != otherTilemap.layoutGrid)
                        throw new ArgumentException("Other tilemap has other grid layout.");

                    tilemap.SetTile(pos, null);
                    Vector3 worldPos = tilemap.CellToWorld(pos);
                    Vector3Int otherPos = otherTilemap.WorldToCell(worldPos);
                    otherTilemap.SetTile(otherPos, Tile.Raw);
                }
                else
                    tilemap.SetTile(pos, Tile.Raw);
            }

            materilaizeCommand?.Execute(Unit.Default);
        }

        public void ResetPosition()
        {
            if (!Position.TryGetValue(out var pos))
                return;

            tilemap.SetTile(pos, null);
            Position = Maybe<Vector3Int>.None;
        }

        public IObservable<Unit> ObserveMaterialize()
        {
            materilaizeCommand ??= new ReactiveCommand<Unit>();
            return materilaizeCommand;
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                ResetPosition();
                linkedGO.IfSome(go => Object.Destroy(go));
                ghostTile.IfSome(tile => Object.Destroy(tile));
                materilaizeCommand?.Dispose();
            }

            disposed = true;
        }
    }
}
