using CCEnvs.Unity._2D;
using CCEnvs.Unity._2D.Locations;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Serialization;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CCEnvs.Unity
{
    public class Location : CCBehaviour, ILocation
    {
        protected readonly Dictionary<string, ILocationLayer> layers = new();

        [SerializeField]
        protected SerializedBoundsInt m_CellBounds;

        private bool refreshScheduled;

        public Result<ILocationLayer> this[string name] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!layers.TryGetValue(name, out ILocationLayer layer))
                    return (null, new KeyNotFoundException($"{nameof(name)}: {name}/"));

                return (layer, null);
            }
        }

        public Result<ILocationLayer> this[Enum key] {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[key.ToString()];
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
        private bool drawDebugBounds;

        private void OnValidate()
        {
            if (drawDebugBounds)
            {
                int xMin = GetCellBounds().xMin;
                int xMax = GetCellBounds().xMax;
                int yMin = GetCellBounds().yMin;
                int yMax = GetCellBounds().yMax;
                Vector3 center = GetCellBounds().center;
                var bottomLeft = new Vector3(xMin, yMin) - center;
                var bottomRight = new Vector3(xMax, yMin) - center;
                var upperLeft = new Vector3(xMin, yMax) - center;
                var upperRight = new Vector3(xMax, yMax) - center;
                var color = Color.cyan;
                var duration = 3f;

                UnityEngine.Debug.DrawLine(bottomLeft, bottomRight, color, duration);
                UnityEngine.Debug.DrawLine(bottomRight, upperRight, color, duration);
                UnityEngine.Debug.DrawLine(upperRight, upperLeft, color, duration);
                UnityEngine.Debug.DrawLine(upperLeft, bottomLeft, color, duration);
            }
        }
#endif //UNITY_EDITOR

        public BoundsInt GetCellBounds() => m_CellBounds.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<ILocationLayer> GetLocationLayer<T>(T key) where T : unmanaged, Enum
        {
            return this[key.ToString()];
        }

        public virtual void Refresh()
        {
            if (refreshScheduled)
                return;

            refreshScheduled = true;
            this.DoActionAsync(static async @this =>
            {
                await UniTask.NextFrame(timing: PlayerLoopTiming.PreUpdate);

                try
                {
                    @this.InitLayers();
                }
                catch (Exception ex)
                {
                    @this.PrintException(ex);
                }
                finally
                {
                    @this.refreshScheduled = false;
                }
            });
        }

        private void InitLayers()
        {
            foreach (var layer in this.Q().ByChildren().Components<ILocationLayer>())
                layers.Add(layer.Name, layer);
        }
    }
}
