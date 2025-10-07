using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.U2D.Locations
{
    public interface ILocation
    {
        Tilemap Map { get; }
        BoundsInt Bounds { get; }
        ICell this[Vector3Int pos] { get; }
        ICell this[Vector2Int pos] { get; }
        ICell this[int x, int y, int z] { get; }
        ICell this[int x, int y] { get; }
    }
    public interface ILocation<out T> : ILocation
        where T : ICell
    {
        new T this[Vector3Int pos] { get; }
        new T this[Vector2Int pos] { get; }
        new T this[int x, int y, int z] { get; }
        new T this[int x, int y] { get; }

        ICell ILocation.this[Vector3Int pos] => this[pos];
        ICell ILocation.this[Vector2Int pos] => this[pos];
        ICell ILocation.this[int x, int y, int z] => this[x, y, z];
        ICell ILocation.this[int x, int y] => this[x, y];
    }
}
