using CCEnvs.Unity._2D;
using CCEnvs.Unity._2D.Locations;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Serialization;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CCEnvs.Unity
{
    public class Location : CCBehaviour, ILocation
    {
        protected readonly Dictionary<string, ILocationLayer> layers = new();

        [SerializeField]
        protected SerializedBoundsInt m_CellBounds;

        public Result<ILocationLayer> this[string name] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!layers.TryGetValue(name, out ILocationLayer layer))
                    return (null, new KeyNotFoundException($"{nameof(name)}: {name}."));

                return (layer, null);
            }
        }

        public Result<ILocationLayer> this[Enum key] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[key.ToString()];
        }

        public BoundsInt CellBounds { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitCellBounds();
        }

        protected override void Start()
        {
            base.Start();
            Refresh();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            Refresh();
        }

#if UNITY_EDITOR
        [Header("Editor")]
        [Space(8f)]
        [SerializeField]
        protected bool drawDebugBounds = true;

        private void OnValidate()
        {
            if (drawDebugBounds)
            {
                int xMin = CellBounds.xMin;
                int xMax = CellBounds.xMax;
                int yMin = CellBounds.yMin;
                int yMax = CellBounds.yMax;
                var bottomLeft = new Vector3(xMin, yMin);
                var bottomRight = new Vector3(xMax, yMin);
                var upperLeft = new Vector3(xMin, yMax);
                var upperRight = new Vector3(xMax, yMax);
                var color = Color.cyan;
                var duration = 3f;

                UnityEngine.Debug.DrawLine(bottomLeft, bottomRight, color, duration);
                UnityEngine.Debug.DrawLine(bottomRight, upperRight, color, duration);
                UnityEngine.Debug.DrawLine(upperRight, upperLeft, color, duration);
                UnityEngine.Debug.DrawLine(upperLeft, bottomLeft, color, duration);
            }
        }
#endif //UNITY_EDITOR

        private static BoundsInt NormalizeBoundsInt(BoundsInt bounds)
        {
            return new BoundsInt(
                bounds.position - bounds.size / 2,
                bounds.size.SetZ(1)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<ILocationLayer> GetLocationLayer<T>(T key) where T : unmanaged, Enum
        {
            return this[key.ToString()];
        }

        public IEnumerator<ILocationLayer> GetEnumerator()
        {
            return layers.Values.GetEnumerator(); ;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [NonSerialized]
        private bool refreshScheduled;
        public virtual void Refresh()
        {
            if (refreshScheduled)
                return;

            refreshScheduled = true;

            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.NextFrame(timing: PlayerLoopTiming.Initialization);

                    try
                    {
                        @this.InitLayers();
                    }
                    finally
                    {
                        @this.refreshScheduled = false;
                    }
                })
                .Forget();
        }

        private void InitLayers()
        {
            foreach (var layer in this.Q().FromChildrens().Components<ILocationLayer>())
                layers.Add(layer.Name, layer);
        }

        private void InitCellBounds()
        {
            BoundsInt bounds = m_CellBounds.Deserialized;

            if (bounds.size.x > 0 || bounds.size.y > 0)
            {
                CellBounds = NormalizeBoundsInt(bounds);
                return;
            }

            foreach (var tilemap in GetComponentsInChildren<Tilemap>())
            {
                if (bounds.size.sqrMagnitude < tilemap.cellBounds.size.sqrMagnitude)
                    bounds = tilemap.cellBounds;
            }

            CellBounds = NormalizeBoundsInt(bounds);
        }
    }
}
