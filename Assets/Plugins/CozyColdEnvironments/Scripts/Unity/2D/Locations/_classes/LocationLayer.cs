using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using Humanizer;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    [RequireComponent(typeof(Tilemap))]
    public abstract class LocationLayer<T> : CCBehaviour, ILocationLayer<T>
        where T : ICell
    {
        [GetByParent(IsOptional = true)]
        private ILocation? m_Location;

        private Dictionary<Vector3Int, T> cells;

        public Result<T> this[Vector3Int pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!cells.TryGetValue(pos, out var cell))
                    return new Result<T>(default, new PointOutOfBoundsException(pos));

                return Result.Create(cell);
            }
        }

        public Result<T> this[Vector2Int pos] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[(Vector3Int)pos];
        }

        public Result<T> this[Vector3 pos] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[ConvertPosition(pos)];
        }

        public Result<T> this[Vector2 pos] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[ConvertPosition(pos)];
        }

        public Result<T> this[int x, int y] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[new Vector3Int(x, y)];
        }

        public Result<T> this[float x, float y] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[new Vector2(x, y)];
        }

        [field: GetBySelf]
        public Tilemap tilemap { get; private set; } = null!;

        public Maybe<ILocation> Location => m_Location.Maybe()!;

        public BoundsInt CellBounds { get; private set; }

        public string Name => name;
        public Maybe<object> Owner { get; private set; }

        protected override void Start()
        {
            base.Start();
            CellBounds = Location.Map(x => x.GetCellBounds()).GetValue(tilemap.cellBounds);
            SetupCellCollection();
            InitCells();
        }

        public static void ReplaceTile(LocationLayer<T> instance,
                                       Vector3Int newPos,
                                       T cellToMove,
                                       T? replacedCell)
        {
            Vector3Int oldPos = cellToMove.Position;

            if (replacedCell.IsNotNull())
            {
                TileBase? replacedTile = replacedCell.GetTile().Raw;

                replacedCell.SetTile(cellToMove.GetTile().Raw);
                cellToMove.SetTile(replacedTile);

                replacedCell.SetPosition(oldPos);
                cellToMove.SetPosition(newPos);

                instance.cells[oldPos] = replacedCell;
                instance.cells[newPos] = cellToMove;
            }
            else
            {
                cellToMove.SetPosition(newPos);
                instance.cells.Remove(oldPos);
                instance.cells[newPos] = cellToMove;
            }
        }

        public bool Contains() => cells.Count > 0;
        public bool Contains(T? cell)
        {
            if (cell.IsNull())
                return false;

            return cells.ContainsValue(cell);
        }
        public bool Contains(Vector3Int pos)
        {
            var boundExt = new BoundsInt(CellBounds.position, CellBounds.size.AddZ(1));
            var t = boundExt.Contains(pos);
            return t;
        }
        public bool Contains(Vector2Int pos)
        {
            return Contains((Vector3Int)pos);
        }
        public bool Contains(Vector3 pos)
        {
            return Contains(ConvertPosition(pos));
        }
        public bool Contains(Vector2 pos)
        {
            return Contains(ConvertPosition(pos));
        }
        public bool Contains(int x, int y)
        {
            return Contains(new Vector3Int(x, y));
        }
        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        public void SetCell(Vector3Int pos, T cell)
        {
            if (!Contains(pos))
                return;
            CC.Guard.IsNotNull(cell, nameof(cell));

            cells[pos] = cell;
            cell.SetTile(cell.GetTile().Raw);
        }
        public void SetCell(Vector2Int pos, T cell)
        {
            SetCell((Vector3Int)pos, cell);
        }
        public void SetCell(Vector3 pos, T cell)
        {
            SetCell(ConvertPosition(pos), cell);
        }
        public void SetCell(Vector2 pos, T cell)
        {
            SetCell(ConvertPosition(pos), cell);
        }
        public void SetCell(int x, int y, T cell)
        {
            SetCell(new Vector3Int(x, y), cell);
        }
        public void SetCell(float x, float y, T cell)
        {
            SetCell(new Vector3(x, y), cell);
        }

        public void MoveCell(Vector3Int from, Vector3Int to)
        {
            if (from == to
                ||
                cells.TryGetValue(from, out T cellToMove)
                ||
                !Contains(to)
                )
                return;

            if (cells.TryGetValue(to, out T replacedCell))
            {
                if (replacedCell.Is<Tile>(out var tileBasic))
                {
                    var repalcedGO = tilemap.GetInstantiatedObject(to);
                    var origGO = tileBasic.gameObject;

                    tileBasic.gameObject = repalcedGO;
                    ReplaceTile(this, to, cellToMove, replacedCell);
                    tileBasic.gameObject = origGO;
                }
                else
                    ReplaceTile(this, to, cellToMove, replacedCell);

                replacedCell.Refresh();
            }
            else
                ReplaceTile(this, to, cellToMove, replacedCell: default);

            cellToMove.Refresh();
        }
        public void MoveCell(Vector2Int from, Vector2Int to)
        {
            MoveCell((Vector3Int)from, (Vector3Int)to);
        }
        public void MoveCell(Vector3 from, Vector3 to)
        {
            MoveCell(ConvertPosition(from), ConvertPosition(to));
        }
        public void MoveCell(Vector2 from, Vector2 to)
        {
            MoveCell(ConvertPosition(from), ConvertPosition(to));
        }
        public void MoveCell(int fromX, int fromY, int toX, int toY)
        {
            MoveCell(new Vector3Int(fromX, fromY), new Vector3Int(toX, toY));
        }
        public void MoveCell(float fromX, float fromY, float toX, float toY)
        {
            MoveCell(new Vector2(fromX, fromY), new Vector2(toX, toY));
        }
        public void MoveCell(T cell, Vector3Int to)
        {
            CC.Guard.IsNotNull(cell, nameof(cell));
            MoveCell(cell.Position, to);
        }
        public void MoveCell(T cell, Vector2Int to)
        {
            MoveCell(cell, (Vector3Int)to);
        }
        public void MoveCell(T cell, Vector3 to)
        {
            MoveCell(cell, ConvertPosition(to));
        }
        public void MoveCell(T cell, Vector2 to)
        {
            MoveCell(cell, ConvertPosition(to));
        }
        public void MoveCell(T cell, int toX, int toY)
        {
            MoveCell(cell, ConvertPosition(new Vector3(toX, toY)));
        }
        public void MoveCell(T cell, float toX, float toY)
        {
            MoveCell(cell, ConvertPosition(new Vector2(toX, toY)));
        }

        public void SetOwner(object? owner) => Owner = owner;

        public virtual Vector3Int ConvertPosition(Vector3 position)
        {
            return tilemap.WorldToCell(position.SetZ(0f));
        }

        public override string ToString()
        {
            return $"{nameof(tilemap)}: {tilemap}; {nameof(Name)}: {Name}; {nameof(Location)}: {Location}; {nameof(CellBounds)}; {CellBounds}";
        }

        protected abstract T CreateCell(Vector3Int pos);

        private void SetupCellCollection()
        {
            int capacity = CellBounds.size.x * CellBounds.size.y * (CellBounds.z + 1);
            cells = new Dictionary<Vector3Int, T>(capacity);
        }

        private void InitCells()
        {
            T cell;
            foreach (var pos in CellBounds.allPositionsWithin)
            {
                cell = CreateCell(pos);
                cells[pos] = cell;

                this.PrintLog($"{cell} inited,");
            }
        }
    }

    public class LocationLayer : LocationLayer<Cell>
    {
        protected override Cell CreateCell(Vector3Int pos)
        {
            return new Cell(this, pos, owner: Owner.Raw);
        }
    }
}