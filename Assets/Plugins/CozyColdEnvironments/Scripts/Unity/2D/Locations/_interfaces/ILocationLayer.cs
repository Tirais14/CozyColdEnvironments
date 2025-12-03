using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using Humanizer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity._2D.Locations
{
    public interface ILocationLayer
    {
        Result<ICell> this[Vector3Int pos] { get; }
        Result<ICell> this[Vector2Int pos] { get; }
        Result<ICell> this[Vector3 pos] { get; }
        Result<ICell> this[Vector2 pos] { get; }
        Result<ICell> this[int x, int y] { get; }
        Result<ICell> this[float x, float y] { get; }

        Tilemap tilemap { get; }
        Maybe<ILocation> Location { get; }
        BoundsInt CellBounds { get; }
        string Name { get; }
        Maybe<object> Owner { get; }

        bool Contains();
        bool Contains(ICell? cell);
        bool Contains(Vector3Int pos);
        bool Contains(Vector2Int pos);
        bool Contains(Vector3 pos);
        bool Contains(Vector2 pos);
        bool Contains(int x, int y);
        bool Contains(float x, float y);

        void SetCell(Vector3Int pos, ICell cell);
        void SetCell(Vector2Int pos, ICell cell);
        void SetCell(Vector3 pos, ICell cell);
        void SetCell(Vector2 pos, ICell cell);
        void SetCell(int x, int y, ICell cell);
        void SetCell(float x, float y, ICell cell);

        void MoveCell(Vector3Int from, Vector3Int to);
        void MoveCell(Vector2Int from, Vector2Int to);
        void MoveCell(Vector3 from, Vector3 to);
        void MoveCell(Vector2 from, Vector2 to);
        void MoveCell(int fromX, int fromY, int toX, int toY);
        void MoveCell(float fromX, float fromY, float toX, float toY);
        void MoveCell(ICell cell, Vector3Int to);
        void MoveCell(ICell cell, Vector2Int to);
        void MoveCell(ICell cell, Vector3 to);
        void MoveCell(ICell cell, Vector2 to);
        void MoveCell(ICell cell, int toX, int toY);
        void MoveCell(ICell cell, float toX, float toY);

        void SetOwner(object? owner);

        Vector3Int ConvertPosition(Vector3 position);
    }
    public interface ILocationLayer<T> : ILocationLayer
        where T : ICell
    {
        new Result<T> this[Vector3Int pos] { get; }
        new Result<T> this[Vector2Int pos] { get; }
        new Result<T> this[Vector3 pos] { get; }
        new Result<T> this[Vector2 pos] { get; }
        new Result<T> this[int x, int y] { get; }
        new Result<T> this[float x, float y] { get; }

        Result<ICell> ILocationLayer.this[Vector3Int pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocationLayer.this[Vector2Int pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocationLayer.this[Vector3 pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocationLayer.this[Vector2 pos] => this[pos].Cast<ICell>();
        Result<ICell> ILocationLayer.this[int x, int y] => this[x, y].Cast<ICell>();
        Result<ICell> ILocationLayer.this[float x, float y] => this[x, y].Cast<ICell>();

        bool Contains(T? cell);

        void SetCell(Vector3Int pos, T cell);
        void SetCell(Vector2Int pos, T cell);
        void SetCell(Vector3 pos, T cell);
        void SetCell(Vector2 pos, T cell);
        void SetCell(int x, int y, T cell);
        void SetCell(float x, float y, T cell);

        void MoveCell(T cell, Vector3Int to);
        void MoveCell(T cell, Vector2Int to);
        void MoveCell(T cell, Vector3 to);
        void MoveCell(T cell, Vector2 to);
        void MoveCell(T cell, int toX, int toY);
        void MoveCell(T cell, float toX, float toY);

        bool ILocationLayer.Contains(ICell? cell)
        {
            return cell.Is<T>(out var typed) && Contains(typed);
        }

        void ILocationLayer.SetCell(Vector3Int pos, ICell cell)
        {
            SetCell(pos, cell.To<T>());
        }
        void ILocationLayer.SetCell(Vector2Int pos, ICell cell)
        {
            SetCell(pos, cell.To<T>());
        }
        void ILocationLayer.SetCell(Vector3 pos, ICell cell)
        {
            SetCell(pos, cell.To<T>());
        }
        void ILocationLayer.SetCell(Vector2 pos, ICell cell)
        {
            SetCell(pos, cell.To<T>());
        }
        void ILocationLayer.SetCell(int x, int y, ICell cell)
        {
            SetCell(x, y, cell.To<T>());
        }
        void ILocationLayer.SetCell(float x, float y, ICell cell)
        {
            SetCell(x, y, cell.To<T>());
        }

        void ILocationLayer.MoveCell(ICell cell, Vector3Int to)
        {
            MoveCell(cell.To<T>(), to);
        }
        void ILocationLayer.MoveCell(ICell cell, Vector2Int to)
        {
            MoveCell(cell.To<T>(), to);
        }
        void ILocationLayer.MoveCell(ICell cell, Vector3 to)
        {
            MoveCell(cell.To<T>(), to);
        }
        void ILocationLayer.MoveCell(ICell cell, Vector2 to)
        {
            MoveCell(cell.To<T>(), to);
        }
        void ILocationLayer.MoveCell(ICell cell, int toX, int toY)
        {
            MoveCell(cell.To<T>(), toX, toY);
        }
        void ILocationLayer.MoveCell(ICell cell, float toX, float toY)
        {
            MoveCell(cell.To<T>(), toX, toY);
        }
    }
}
