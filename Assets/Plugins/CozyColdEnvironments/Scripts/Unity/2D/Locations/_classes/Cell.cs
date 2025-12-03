using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class Cell : ICell, IDisposable
    {
        private Maybe<TileBase> tile = null!;
        private readonly ReactiveProperty<Vector3Int> position = new();

        public ILocationLayer LocationLayer { get; }
        public Vector3Int Position {
            get => position.Value;
            set => position.Value = value;  
        }
        public Maybe<object> Owner { get; private set; }

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
        public Maybe<TileBase> GetTile() => tile;

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
            this.tile = tile;
            LocationLayer.tilemap.SetTile(Position, tile);
        }

        public bool RemoveTile([NotNullWhen(true)] out TileBase? tile)
        {
            if (this.tile.TryGetValue(out tile))
            {
                SetTile(null);
                return true;
            }

            return false;
        }
        public bool RemoveTile() => RemoveTile(out _);

        public Bounds GetBounds() => LocationLayer.tilemap.GetBoundsLocal(Position);

        public bool HasTile() => tile.IsSome;

        public bool HasOwner() => Owner.IsSome;
        public bool HasOwner(object owner) => Owner.Has(owner);

        public bool SetOwner(object owner)
        {
            Owner = owner;
            return true;
        }

        public void Refresh()
        {
            tile = LocationLayer.tilemap.GetTile(Position);
        }

        public void SetPosition(Vector3Int pos)
        {
            Position = pos;
        }

        public override string ToString()
        {
            return $"{nameof(tile)}: {tile}; {nameof(Position)}: {Position}; {nameof(LocationLayer)}: {LocationLayer}; {nameof(Owner)}: {Owner}";
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

        public IObservable<Vector3Int> ObservePosition() => position;
    }
}
