using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using static UnityEngine.Tilemaps.Tilemap;

#nullable enable
namespace UTIRLib.TwoD.Map
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tilemap))]
    public class LocationLayer : MonoX, ILocationLayer
    {
        private Map<ILocationCell> cellMap = null!;

        [GetSelf]
        private Tilemap tilemap = null!;

        public BoundsInt Bounds => Parent.Bounds;

        [GetByParent]
        public ILocation Parent { get; private set; } = null!;

        public event Action<SyncTile[]>? OnTileChanged;

        public ILocationCell this[Vector3Int pos] {
            get => GetCell(pos);
            set => SetCell(pos, value);
        }

        protected override void OnStart()
        {
            base.OnStart();

            tilemapTileChanged += OnTilemapTileChanged;
        }

        public ILocationCell GetCell(Vector3Int pos)
        {
            if (!InBounds(pos))
                throw new ArgumentOutOfRangeException(nameof(pos));

            return cellMap[pos];
        }

        public bool InBounds(Vector3Int pos)
        {
            return Parent.InBounds(pos);
        }

        public bool RemoveCell(Vector3Int pos)
        {
            if (!InBounds(pos))
                return false;

            cellMap[pos].Tile = null;

            return true;
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void SetCell(Vector3Int pos, ILocationCell? cell)
        {
            if (InBounds(pos))
                throw new ArgumentOutOfRangeException(nameof(pos));

            if (cell.IsNull())
            {
                RemoveCell(pos);

                return;
            }

            tilemap.SetTile(pos, cell.Tile);
        }

        public void Dispose()
        {
            tilemapTileChanged -= OnTilemapTileChanged;
        }

        private void OnTilemapTileChanged(Tilemap tilemap, SyncTile[] syncTiles)
        {
            if (tilemap == this.tilemap && OnTileChanged is not null)
            {
                for (int i= 0; i < syncTiles.Length; i++)
                {
                    SyncTile[] changedTilesInBounds = syncTiles.Where(x => InBounds(x.position))
                                                               .ToArray();

                    OnTileChanged(changedTilesInBounds);
                }
            }
        }

        private void OnDisable() => Dispose();
    }
}
