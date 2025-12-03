using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class Cell : ICell, IDisposable
    {
        private readonly ReactiveProperty<Vector3Int> position = new();

        public ILocationLayer LocationLayer { get; }
        public Vector3Int Position {
            get => position.Value;
            set => position.Value = value;  
        }
        public Maybe<object> Owner { get; private set; }
        public Tilemap tilemap => LocationLayer.tilemap;

        public Cell(
            ILocationLayer locationLayer,
            Vector3Int position,
            object? owner = null)
        {
            CC.Guard.IsNotNull(locationLayer, nameof(locationLayer));

            LocationLayer = locationLayer;
            Position = position;
            Owner = owner;

            Refresh();
        }

        public Cell(
            ILocationLayer locationLayer,
            Vector2Int position,
            object? owner = null)
            :
            this(locationLayer, (Vector3Int)position, owner: owner)
        {
        }
        public Cell(
            ILocationLayer locationLayer,
            Vector3 position,
            object? owner = null)
            :
            this(locationLayer, locationLayer.ConvertPosition(position), owner: owner)
        {
        }

        public Cell(
            ILocationLayer locationLayer,
            Vector2 position,
            object? owner = null)
            :
            this(locationLayer, locationLayer.ConvertPosition(position), owner: owner)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TileBase> GetTile() => tilemap.GetTile(Position);

        public Maybe<T> GetTile<T>() where T : TileBase
        {
            return GetTile().Raw.As<T>();
        }

        public Maybe<GameObject> GetInstantiatedGameObject()
        {
            return LocationLayer.tilemap.GetInstantiatedObject(Position);
        }

        public void SetTile(TileBase? tile)
        {
            LocationLayer.tilemap.SetTile(Position, tile);
        }

        public bool RemoveTile([NotNullWhen(true)] out TileBase? tile)
        {
            if (GetTile().TryGetValue(out tile))
            {
                SetTile(null);
                return true;
            }

            return false;
        }
        public bool RemoveTile() => RemoveTile(out _);

        public Bounds GetBounds() => LocationLayer.tilemap.GetBoundsLocal(Position);

        public bool HasTile() => GetTile().IsSome;

        public bool HasOwner() => Owner.IsSome;
        public bool HasOwner(object owner) => Owner.Has(owner);

        public bool SetOwner(object owner)
        {
            Owner = owner;
            return true;
        }

        public void Refresh()
        {
            //TODO:
        }

        public void SetPosition(Vector3Int pos)
        {
            Position = pos;
        }

        public override string ToString()
        {
            return $"Tile: {GetTile().Raw}; {nameof(Position)}: {Position}; {nameof(Owner)}: {Owner}";
        }

        [NonSerialized]
        private bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                position.Dispose();

            disposed = true;
        }

        public GhostCell ToGhost(Tilemap? tilemap = null)
        {
            return new GhostCell(this, tilemap.Maybe().GetValue(this.tilemap));
        }

        public Maybe<TileData> GetTileData()
        {
            if (!GetTile().TryGetValue(out TileBase? tile))
                return Maybe<TileData>.None;

            TileData tileData = default;
            tile.GetTileData(Position, LocationLayer.tilemap, ref tileData);
            return tileData;
        }

        public Maybe<Sprite> GetTileSprite()
        {
            return GetTileData().Map(x => x.sprite);
        }

        public Maybe<GameObject> GetTilePrefab()
        {
            return GetTileData().Map(x => x.gameObject);
        }

        public IObservable<Vector3Int> ObservePosition() => position;
    }
}
