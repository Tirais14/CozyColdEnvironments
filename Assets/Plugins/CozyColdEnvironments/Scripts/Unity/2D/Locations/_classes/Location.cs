using CCEnvs.Diagnostics;
using CCEnvs.Unity._2D;
using CCEnvs.Unity._2D.Locations;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Serialization;
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
            if (refreshScheduled)
                return;

            refreshScheduled = true;
            OnNextFrame(this, static @this =>
            {
                try
                {
                    @this.Refresh();
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

        public BoundsInt GetCellBounds() => m_CellBounds.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<ILocationLayer> GetLocationLayer<T>(T key) where T : unmanaged, Enum
        {
            return this[key.ToString()];
        }

        public virtual void Refresh()
        {
            InitLayers();
        }

        private void InitLayers()
        {
            foreach (var layer in this.Q().ByChildren().Components<ILocationLayer>())
                layers.Add(layer.Name, layer);
        }
    }
}
