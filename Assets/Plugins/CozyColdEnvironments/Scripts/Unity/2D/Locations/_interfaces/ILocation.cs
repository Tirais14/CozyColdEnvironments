using CCEnvs.TypeMatching;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.U2D.Locations
{
    public interface ILocation
    {
        Tilemap tilemap { get; }
        BoundsInt Bounds { get; }
        Result<ICell> this[Vector3Int pos] { get; }
        Result<ICell> this[Vector2Int pos] { get; }
        Result<ICell> this[Vector3 pos] { get; }
        Result<ICell> this[Vector2 pos] { get; }
        Result<ICell> this[int x, int y] { get; }
        Result<ICell> this[float x, float y] { get; }

        bool Contains();
        bool Contains(ICell? cell);
        bool Contains(Vector3Int pos);
        bool Contains(Vector2Int pos);
        bool Contains(Vector3 pos);
        bool Contains(Vector2 pos);
        bool Contains(int x, int y);
        bool Contains(float x, float y);
    }
    public interface ILocation<T> : ILocation
        where T : ICell
    {
        new Result<T> this[Vector3Int pos] { get; }
        new Result<T> this[Vector2Int pos] { get; }
        new Result<T> this[Vector3 pos] { get; }
        new Result<T> this[Vector2 pos] { get; }
        new Result<T> this[int x, int y] { get; }
        new Result<T> this[float x, float y] { get; }

        Result<ICell> ILocation.this[Vector3Int pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocation.this[Vector2Int pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocation.this[Vector3 pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocation.this[Vector2 pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocation.this[int x, int y] => this[x, y].Cast<ICell>();
        Result<ICell> ILocation.this[float x, float y] => this[x, y].Cast<ICell>();

        bool Contains(T? cell);

        bool ILocation.Contains(ICell? cell)
        {
            return cell.Is<T>(out var typed) && Contains(typed);
        }
    }
}
