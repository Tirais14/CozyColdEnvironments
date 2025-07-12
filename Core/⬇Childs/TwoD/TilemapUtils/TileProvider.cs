using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace UTIRLib.TwoD
{
    [Serializable]
    public class TileProvider : ITileProvider
    {
        [SerializeField] private TileBase? tile;

        public TileBase? Tile { get => tile; set => SetTile(value); }
        public bool HasTile => Tile != null;

        public TileProvider()
        {
        }

        public TileProvider(TileBase? tile) => this.tile = tile;

        public void SetTile(TileBase? tile) => this.tile = tile;

        public TTile? GetTile<TTile>() where TTile : TileBase => Tile as TTile;

        public bool TryGetTile<TTile>([NotNullWhen(true)] out TTile? tile) where TTile : TileBase
        {
            tile = GetTile<TTile>();

            return tile != null;
        }

        public static implicit operator TileBase?(TileProvider tileProvider) => tileProvider.tile;
    }

    [Serializable]
    public class TileProvider<T> : ITileProvider<T>
        where T : TileBase
    {
        [SerializeField] protected T? tile;

        public T? Tile { get => tile; set => SetTile(value); }
        public bool HasTile => Tile != null;

        TileBase? IReadOnlyTileProvider.Tile => Tile;
        TileBase? ITileProvider.Tile { get => Tile; set => SetTile(value); }

        public TileProvider()
        {
        }

        public TileProvider(T? tile) => this.tile = tile;

        public void SetTile(T? tile) => this.tile = tile;

        public void SetTile(TileBase? tile) => SetTile(tile as T);

        public TTile? GetTile<TTile>() where TTile : TileBase => Tile as TTile;

        public bool TryGetTile<TTile>([NotNullWhen(true)] out TTile? tile) where TTile : TileBase
        {
            tile = GetTile<TTile>();

            return tile != null;
        }
    }
}