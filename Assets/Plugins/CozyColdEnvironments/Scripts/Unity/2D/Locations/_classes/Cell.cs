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
        private readonly ReactiveProperty<Vector3Int> position = new();

        private ReactiveCommand<TileBase>? setTileCmd;
        private ReactiveCommand<Unit>? removeTileCmd;
        private Maybe<IDisposable> tileSubscription;
        private bool tileSubbed;

        public ILocationLayer LocationLayer { get; }
        public Vector3Int Position {
            get => position.Value;
            set => position.Value = value;  
        }
        public Maybe<object> Owner { get; private set; }
        public Tilemap tilemap { get; }

        public Cell(
            ILocationLayer locationLayer,
            Vector3Int position,
            object? owner = null)
        {
            CC.Guard.IsNotNull(locationLayer, nameof(locationLayer));

            LocationLayer = locationLayer;
            tilemap = locationLayer.tilemap;
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
            tileSubscription.IfSome(x => x.Dispose());
            tilemap.SetTile(Position, tile);
            SubscribeTile(tile);
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
            Position = pos;
        }

        public override string ToString()
        {
            return $"Tile: {GetTile().Raw}; {nameof(Position)}: {Position}; {nameof(Owner)}: {Owner}";
        }

        public Maybe<GhostCell> ToGhost(Tilemap? tilemap = null)
        {
            if (GetTile().IsNone)
                return Maybe<GhostCell>.None;

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

        public IObservable<TileBase> ObserveSetTile()
        {
            setTileCmd ??= new ReactiveCommand<TileBase>();
            return setTileCmd;
        }

        public IObservable<Unit> ObserveRemoveTile()
        {
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
    }
}
