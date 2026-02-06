using CCEnvs.FuncLanguage;
using System;
using System.Diagnostics.CodeAnalysis;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public interface ICell
    {
        Vector3Int Position { get; }
        ILocationLayer LocationLayer { get; }
        Maybe<object> Owner { get; }
        Tilemap tilemap { get; }

        Maybe<TileBase> GetTile();
        Maybe<T> GetTile<T>() where T : TileBase;
        Maybe<GameObject> GetInstantiatedGameObject();

        void SetTile(TileBase? tile);

        bool RemoveTile();
        bool RemoveTile([NotNullWhen(true)] out TileBase? result);

        Bounds GetBounds();

        bool HasTile();

        bool HasOwner();
        bool HasOwner(object owner);

        bool SetOwner(object owner);

        void Refresh();

        void SetPosition(Vector3Int pos);

        Maybe<Sprite> GetTileSprite();

        Maybe<GameObject> GetTilePrefab();

        Maybe<GhostCell> ToGhost(Tilemap? tilemap = null);

        public Maybe<TileData> GetTileData();

        Observable<Vector3Int> ObservePosition();

        /// <summary>
        /// Doesn't trigger when tile setted outside of <see cref="ICell"/>
        /// </summary>
        Observable<TileBase> ObserveSetTile();

        /// <summary>
        /// Doesn't trigger when tile setted outside of <see cref="ICell"/>
        /// </summary>
        Observable<Unit> ObserveRemoveTile();
    }
}
