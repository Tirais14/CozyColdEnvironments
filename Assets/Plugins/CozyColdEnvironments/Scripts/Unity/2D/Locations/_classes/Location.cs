using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Serialization;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.U2D.Locations
{
    [RequireComponent(typeof(Tilemap))]
    public class Location<T> : CCBehaviour, ILocation<T>
        where T : ICell
    {
        [SerializeField]
        [Tooltip("If none will be used Tilemap.cellBounds")]
        protected Maybe<SerializedBoundsInt> _bounds;

        private MapInt<T> cells;

        public Result<T> this[Vector3Int pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!Contains(pos))
                    return new Result<T>(default, new LocationOutOfBoundsException(pos));

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
            get => this[tilemap.WorldToCell(pos)];
        }

        public Result<T> this[Vector2 pos] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[tilemap.WorldToCell(pos)];
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

        public BoundsInt Bounds {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _bounds.Map(x => (BoundsInt)x).GetValue(tilemap.cellBounds);
        }

        [field: GetBySelf]
        public Tilemap tilemap {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        } = null!;

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
            cells = new MapInt<T>(Bounds);
        }

        protected override void Start()
        {
            base.Start();
            InitCells();
        }

        private void InitCells()
        {
            T cell;
            foreach (var pos in Bounds.allPositionsWithin)
            {
                cell = typeof(T).Reflect()
                                .NonPublic()
                                .Arguments(this, pos)
                                .Cache()
                                .CreateInstance<T>();

                cells[pos] = cell;
                this.PrintLog($"Cell inited; position: {pos}; tile: {cell.GetTile().Map(x => x.name).GetValue("none")}.");
            }
        }
    }

    public class Location : Location<Cell>
    {
    }
}