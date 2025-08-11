using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tilemap;

#nullable enable
namespace UTIRLib.TwoD.Map
{
    public class LocationCell : ILocationCell
    {
        private TileBase? tile;
        private Tilemap tilemap = null!;

        public Vector3Int Position { get; set; }
        public ILocationLayer Parent { get; private set; } = null!;
        public TileBase? Tile {
            get => tile;
            set
            {
                tilemap.SetTile(Position, value);

                OnTileChanged?.Invoke(this);
            }
        }
        public bool HasTile => Tile != null;

        public event Action<ILocationCell>? OnTileChanged;

        public LocationCell(Tilemap tilemap,
                            Vector3Int pos,
                            ILocationLayer sourceLayer,
                            TileBase? tile = null)
        {
            this.tilemap = tilemap;
            Position = pos;
            Parent = sourceLayer;
            Tile = tile;

            Parent.OnTileChanged += OnParentTileChanged;
        }

        public void Dispose()
        {
            Parent.OnTileChanged -= OnParentTileChanged;
            OnTileChanged = null;
        }

        private void OnParentTileChanged(SyncTile[] tiles)
        {
            int tilesCount = tiles.Length;
            for (int i = 0; i < tilesCount; i++)
            {
                if (tiles[i].position == Position)
                {
                    tile = tiles[i].tile;

                    OnTileChanged?.Invoke(this);
                }
            }
        }
    }
}
