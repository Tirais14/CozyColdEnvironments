using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
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
                if (!Contains(pos))
                    return new Result<T>(default, new PointOutOfBoundsException(pos));

                return Result.Lazy(() => cells[pos]);
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
        public Tilemap tilemap {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        } = null!;

        public Maybe<ILocation> Location => m_Location.Maybe()!;

        public BoundsInt CellBounds {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Location.Map(static loc => loc.GetCellBounds()).GetValue(tilemap.cellBounds);
            }
        }

        public string Name => name;
        public Maybe<object> Owner { get; private set; }

        protected override void Start()
        {
            base.Start();
            cells = new Dictionary<Vector3Int, T>(CellBounds.size.x * CellBounds.size.y * (CellBounds.z + 1));
            InitCells();
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
                throw new PointOutOfBoundsException(pos);

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
            if (!cells.TryGetValue(from, out T cellToMove))
                return;

            if (cells.TryGetValue(to, out T replacedCell))
            {
                if (replacedCell.Is<Tile>(out var tileBasic))
                {
                    var repalcedGO = tilemap.GetInstantiatedObject(to);
                    var origGO = tileBasic.gameObject;
                    tileBasic.gameObject = repalcedGO;

                }
            }
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

        public void SetOwner(object? owner) => Owner = owner;

        public virtual Vector3Int ConvertPosition(Vector3 position)
        {
            return tilemap.WorldToCell(position.SetZ(0f));
        }

        public override string ToString()
        {
            return $"{nameof(tilemap)}: {tilemap}; {nameof(Name)}: {Name}; {nameof(Location)}: {Location}; {nameof(CellBounds)}; {CellBounds}";
        }

        public virtual void OnCellMoved(Vector3Int from, Vector3Int to)
        {
            (cells[from], cells[to]) = (cells[to], cells[from]);
        }

        protected abstract T CreateCell(Vector3Int pos, TileBase? tile);

        private void InitCells()
        {
            T cell;
            foreach (var pos in CellBounds.allPositionsWithin)
            {
                cell = CreateCell(pos, tilemap.GetTile(pos));
                cells[pos] = cell;

                this.PrintLog($"{cell} inited,");
            }
        }
    }

    public class LocationLayer : LocationLayer<Cell>
    {
        protected override Cell CreateCell(Vector3Int pos, TileBase? tile)
        {
            return new Cell(this, pos, tile: tile, owner: Owner.Raw);
        }
    }
}