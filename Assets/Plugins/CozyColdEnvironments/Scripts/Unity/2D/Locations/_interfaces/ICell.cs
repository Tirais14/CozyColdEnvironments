using CCEnvs.FuncLanguage;
using System.Diagnostics.CodeAnalysis;
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

        Maybe<TileBase> GetTile();
        Maybe<T> GetTile<T>() where T : TileBase;

        void SetTile(TileBase? tile);

        bool RemoveTile();
        bool RemoveTile([NotNullWhen(true)] out TileBase? result);

        Bounds GetBounds();

        bool HasTile();

        bool HasOwner();
        bool HasOwner(object owner);

        bool SetOwner(object owner);

        void Refresh();
    }
}
