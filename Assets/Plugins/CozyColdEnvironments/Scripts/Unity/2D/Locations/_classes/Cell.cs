using CCEnvs.FuncLanguage;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public class Cell : ICell, IDisposable
    {
        private readonly ILocationLayer locationLayer;
        private readonly ReactiveProperty<Vector3Int> position = new();
        private readonly Tilemap _tilemap;

        private Maybe<object> owner;
        private ReactiveCommand<TileBase>? setTileCmd;
        private ReactiveCommand<Unit>? removeTileCmd;
        private Maybe<IDisposable> tileSubscription;
        private bool tileSubbed;

        public ILocationLayer LocationLayer {
            get
            {
                Validate();
                return locationLayer;
            }
        }
        public Vector3Int Position {
            get
            {
                Validate();
                return position.Value;
            }
            set
            {
                Validate();
                position.Value = value;
            } 
        }
        public Maybe<object> Owner {
            get
            {
                Validate();
                return owner;
            }
        }
        public Tilemap tilemap {
            get
            {
                Validate();
                return _tilemap;
            }
        }

        public Cell(
            ILocationLayer locationLayer,
            Vector3Int position,
            object? owner = null)
        {
            CC.Guard.IsNotNull(locationLayer, nameof(locationLayer));

            this.locationLayer = locationLayer;
            _tilemap = locationLayer.tilemap;
            Position = position;
            this.owner = owner;

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
        public Maybe<TileBase> GetTile()
        {
            Validate();

            if (tilemap == null)
                return Maybe<TileBase>.None;

            return tilemap.GetTile(Position);
        }

        public Maybe<T> GetTile<T>() where T : TileBase
        {
            Validate();
            return GetTile().Raw.As<T>();
        }

        public Maybe<GameObject> GetInstantiatedGameObject()
        {
            Validate();
            return LocationLayer.tilemap.GetInstantiatedObject(Position);
        }

        public void SetTile(TileBase? tile)
        {
            Validate();

            tileSubscription.IfSome(x => x.Dispose());
            tilemap.SetTile(Position, tile);

            SubscribeTile(tile);
        }

        public bool RemoveTile([NotNullWhen(true)] out TileBase? tile)
        {
            Validate();

            if (GetTile().TryGetValue(out tile))
            {
                SetTile(null);
                return true;
            }

            return false;
        }
        public bool RemoveTile()
        {
            Validate();
            return RemoveTile(out _);
        }

        public Bounds GetBounds()
        {
            Validate();
            return LocationLayer.tilemap.GetBoundsLocal(Position);
        }

        public bool HasTile()
        {
            Validate();
            return GetTile().IsSome;
        }

        public bool HasOwner()
        {
            Validate();
            return Owner.IsSome;
        }
        public bool HasOwner(object owner)
        {
            Validate();
            return Owner.Has(owner);
        }

        public bool SetOwner(object owner)
        {
            Validate();
            this.owner = owner;
            return true;
        }

        public void Refresh()
        {
            Validate();

            if (GetInstantiatedGameObject().TryGetValue(out var goInstance)
                &&
                GetTile().TryGetValue(out var tile)
                &&
                !tileSubbed)
            {
                SubscribeTile(tile);
            }
        }

        public void SetPosition(Vector3Int pos)
        {
            Validate();
            Position = pos;
        }

        public override string ToString()
        {
            return $"Tile: {GetTile().Raw}; {nameof(Position)}: {Position}; {nameof(Owner)}: {Owner}";
        }

        public Maybe<GhostCell> ToGhost(Tilemap? tilemap = null)
        {
            Validate();

            if (GetTile().IsNone)
                return Maybe<GhostCell>.None;

            return new GhostCell(this, tilemap.Maybe().GetValue(this.tilemap));
        }

        public Maybe<TileData> GetTileData()
        {
            Validate();

            if (!GetTile().TryGetValue(out TileBase? tile))
                return Maybe<TileData>.None;

            TileData tileData = default;
            tile.GetTileData(Position, LocationLayer.tilemap, ref tileData);
            return tileData;
        }

        public Maybe<Sprite> GetTileSprite()
        {
            Validate();
            return GetTileData().Map(x => x.sprite);
        }

        public Maybe<GameObject> GetTilePrefab()
        {
            Validate();
            return GetTileData().Map(x => x.gameObject);
        }

        public IObservable<Vector3Int> ObservePosition()
        {
            Validate();
            return position;
        }

        public IObservable<TileBase> ObserveSetTile()
        {
            Validate();
            setTileCmd ??= new ReactiveCommand<TileBase>();
            return setTileCmd;
        }

        public IObservable<Unit> ObserveRemoveTile()
        {
            Validate();
            removeTileCmd ??= new ReactiveCommand<Unit>();
            return removeTileCmd;
        }

        [NonSerialized]
        private bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                position.Dispose();
                tileSubscription.IfSome(x => x.Dispose());
                setTileCmd?.Dispose();
                removeTileCmd?.Dispose();
            }

            disposed = true;
        }

        protected void SubscribeTile(TileBase? tile)
        {
            if (tile == null
                ||
                !GetInstantiatedGameObject().TryGetValue(out var goInstance))
            {
                tileSubbed = false;
                return;
            }

            tileSubscription = goInstance.OnDestroyAsObservable()
                .SubscribeWithState(this,
                static (_, @this) =>
                {
                    @this.tileSubscription.IfSome(x => x.Dispose());

                    UniTask.Create(@this,
                        static async @this =>
                        {
                            await UniTask.NextFrame(PlayerLoopTiming.Initialization);
                            @this.RemoveTile();
                        })
                        .Forget();
                })
                .Maybe();

            tileSubbed = true;
        }

        protected void Validate()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
