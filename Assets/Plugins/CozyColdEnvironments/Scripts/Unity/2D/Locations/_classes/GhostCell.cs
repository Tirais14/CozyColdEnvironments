using CCEnvs.FuncLanguage;
using System;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class GhostCell : IGhostCell
    {
        private Maybe<Tile> ghostTile;
        private Maybe<GameObject> ghostGameObject;
        private ReactiveCommand<MaterializedCellInfo>? materilaizeCommand;
        private ReactiveCommand<GameObject>? ghostGameObjectInstantiated;

        public Maybe<TileBase> Tile { get; }
        public Tilemap tilemap { get; }
        public Maybe<Vector3Int> Position { get; private set; }

        public GhostCell(TileBase? tile, Tilemap tilemap)
        {
            CC.Guard.IsNotNull(tilemap, nameof(tilemap));  

            Tile = tile;
            this.tilemap = tilemap;
        }

        public GhostCell(ICell cell, Tilemap tilemap)
            :
            this(cell.GetTile().Raw, tilemap)
        {
        }

        public void SetPosition(Vector3Int pos)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (Position.Has(pos))
                return;

            if (this.ghostTile.IsNone && Tile.IsSome)
                InstantiateGhostTile();

            if (ghostGameObject.IsNone)
                InstantiatedGhostGameObject();

            if (!this.ghostTile.TryGetValue(out Tile? ghostTile))
                return;

            if (Position.TryGetValue(out Vector3Int previousPos))
                tilemap.SetTile(previousPos, tile: null);

            if (ghostGameObject.TryGetValue(out GameObject? go))
                go.transform.position = tilemap.GetCellCenterWorld(pos);

            tilemap.SetTile(pos, ghostTile);
            Position = pos;
        }

        public MaterializedCellInfo Materialize(Tilemap? otherTilemap = null)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (Position.IsNone)
                return default;

            MaterializedCellInfo materializedCellInfo = default;
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

                    materializedCellInfo = new MaterializedCellInfo(otherTilemap, Tile.Raw, otherPos);
                }
                else
                {
                    tilemap.SetTile(pos, Tile.Raw);
                    materializedCellInfo = new MaterializedCellInfo(tilemap, Tile.Raw, pos);
                }

                materilaizeCommand?.Execute(materializedCellInfo);
            }

            return materializedCellInfo;
        }

        public void ResetPosition()
        {
            if (!Position.TryGetValue(out var pos))
                return;

            tilemap.SetTile(pos, null);
            Position = Maybe<Vector3Int>.None;
        }

        public Observable<MaterializedCellInfo> ObserveMaterialize()
        {
            materilaizeCommand ??= new ReactiveCommand<MaterializedCellInfo>();
            return materilaizeCommand;
        }

        ///<summary>Called before first <see cref="SetPosition(Vector3Int)"/></summary>
        public Observable<GameObject> ObserveGhostGameObjectInstantiated()
        {
            ghostGameObjectInstantiated ??= new ReactiveCommand<GameObject>();
            return ghostGameObjectInstantiated;
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
                ghostGameObject.IfSome(go => Object.Destroy(go));
                ghostTile.IfSome(tile => Object.Destroy(tile));
                materilaizeCommand?.Dispose();
                ghostGameObjectInstantiated?.Dispose();
            }

            disposed = true;
        }

        private static void DisableGameObjectLogic(GameObject go)
        {
            GameObject[] childs = go.Q().FromChildrens().GameObjects().ToArray();

            foreach (var child in childs)
                child.layer = Physics.IgnoreRaycastLayer;

            foreach (var cmp in childs.SelectMany(x => x.GetComponents<Component>()))
            {
                switch (cmp)
                {
                    case MonoBehaviour mono:
                        mono.enabled =false;
                        break;
                    case Collider col:
                        col.enabled = false;
                        break;
                    case Collider2D col2:
                        col2.enabled = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void InstantiatedGhostGameObject()
        {
            if (Tile.TryGetValue(out var tile)
                &&
                this.ghostTile.TryGetValue(out var ghostTile)
                &&
                tile.GetTileGameObject().TryGetValue(out var prefab))
            {
                var ghostGameObject = Object.Instantiate(
                    prefab,
                    ghostTile.transform.GetPosition(),
                    Quaternion.identity,
                    tilemap.transform
                    );

                this.ghostGameObject = ghostGameObject;
                DisableGameObjectLogic(ghostGameObject);

                if (ghostGameObjectInstantiated is not null)
                {
                    try
                    {
                        ghostGameObjectInstantiated?.Execute(ghostGameObject);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }
                }
            }
        }

        private void InstantiateGhostTile()
        {
            if (Tile.TryGetValue(out var tile))
            {
                var ghostTile = ScriptableObject.CreateInstance<Tile>();
                ghostTile.name = $"GhostTile({tile.name})";
                ghostTile.sprite = tile.GetTileSprite().Raw;
                ghostTile.hideFlags = tile.hideFlags;

                this.ghostTile = ghostTile;
            }
        }
    }
}
