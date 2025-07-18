using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace UTIRLib.TwoD.Map
{
    public interface ILocationLayer : IDisposable
    {
        BoundsInt Bounds { get; }
        ILocation Parent { get; }

        event Action<Tilemap.SyncTile[]>? OnTileChanged;

        ILocationCell this[Vector3Int pos] { get; set; }

        void SetCell(Vector3Int pos, ILocationCell cell);

        ILocationCell GetCell(Vector3Int pos);

        bool RemoveCell(Vector3Int pos);

        bool InBounds(Vector3Int pos);
    }
}
