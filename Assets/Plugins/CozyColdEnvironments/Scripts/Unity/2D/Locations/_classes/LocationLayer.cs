using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    [RequireComponent(typeof(Tilemap))]
    public class LocationLayer<T> : CCBehaviour, ILocationLayer<T>
        where T : ICell
    {
        [GetByParent(IsOptional = true)]
        private ILocation? m_Location;

        private MapInt<T> cells;

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
                return Location.Match(some: loc => loc.GetCellBounds(),
                                      none: () => tilemap.cellBounds);
            }
        }

        public string Name => name;

        public bool Contains() => cells.Count > 0;
        public bool Contains(T? cell)
        {
            if (cell.IsNull())
                return false;

            return cells.Contains(cell);
        }
        public bool Contains(Vector3Int pos) => cells.Contains(pos);
        public bool Contains(Vector2Int pos) => cells.Contains(pos);
        public bool Contains(Vector3 pos)
        {
            var t = tilemap.WorldToCell(pos);
            return Contains(t);
        }
        public bool Contains(Vector2 pos)
        {
            var t = tilemap.WorldToCell(pos);
            return Contains(t);
        }
        public bool Contains(int x, int y)
        {
            return Contains(new Vector3Int(x, y));
        }
        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        protected override void Awake()
        {
            base.Awake();
            cells = new MapInt<T>(CellBounds);
        }

        protected override void Start()
        {
            base.Start();
            InitCells();
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

        private void InitCells()
        {
            T cell;
            foreach (var pos in CellBounds.allPositionsWithin)
            {
                cell = typeof(T).Reflect()
                                .NonPublic()
                                .Arguments((typeof(ILocationLayer), this), (typeof(Vector3Int), pos))
                                .Cache()
                                .CreateInstance<T>();

                cells[pos] = cell;
                this.PrintLog($"Cell inited; position: {pos}; tile: {cell.GetTile().Map(x => x.name).GetValue("none")}.");
            }
        }

        private Vector3Int ConvertPosition(Vector3 pos)
        {
            return tilemap.WorldToCell(pos);
        }
    }

    public class LocationLayer : LocationLayer<Cell>
    {
    }
}