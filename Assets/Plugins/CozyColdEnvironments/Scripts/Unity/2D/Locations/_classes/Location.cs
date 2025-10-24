using CCEnvs.Diagnostics;
using CCEnvs.Language;
using CCEnvs.Reflection.Data;
using CCEnvs.Unity;
using CCEnvs.Unity.Collections;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CCEnvs.U2D.Locations
{
    [RequireComponent(typeof(Tilemap))]
    public class Location<T> : CCBehaviour, ILocation<T>
        where T : ICell
    {
        [SerializeField]
        [Tooltip("If HasValue == false - use Tilemap.cellBounds")]
        protected GhostStruct<SerializedBoundsInt> _bounds;

        private MapInt<T> cells;

        [field: GetBySelf]
        public Tilemap Map {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set; 
        } = null!;

        public BoundsInt Bounds {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _bounds.Map(x => (BoundsInt)x).Value(Map.cellBounds);
        }

        public T this[Vector3Int pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => cells[pos];
        }

        public T this[Vector2Int pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => cells[pos];
        }

        public T this[int x, int y, int z] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => cells[x, y, z];
        }

        public T this[int x, int y] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => cells[x, y];
        }

        protected override void Start()
        {
            base.Start();

            cells = new MapInt<T>(Bounds);
            InitCells();
        }

        private void InitCells()
        {
            T cell;
            foreach (var pos in Bounds.allPositionsWithin)
            {
                cell = InstanceFactory.Create<T>(
                    new ExplicitArguments(
                        new ExplicitArgument(this),
                        new ExplicitArgument(pos)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);

                cells[pos] = cell;

                this.PrintLog($"Cell inited; position: {pos}; tile: {cell.GetTile().IfNotDefault(x => x.name, (_) => "none")}.");
            }
        }
    }
    public class Location : Location<Cell>
    {
    }
}
