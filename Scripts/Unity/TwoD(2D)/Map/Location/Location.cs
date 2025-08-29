using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Linq;

#nullable enable
namespace UTIRLib.TwoD.Map
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Grid))]
    public class Location : MonoX, ILocation
    {
        private ILocationLayer[] layers = null!;

        [field: SerializeField]
        public BoundsInt Bounds { get; private set; }

        public ILocationLayer this[int index] => layers[index];
        public int LayersCount => layers.Length;

        protected override void OnAwake()
        {
            base.OnAwake();

            SetLayers();
        }

        public ILocationLayer GetLayer(int index) => layers[index];

        public T? GetLayer<T>(int index) where T : ILocationLayer
        {
            return GetLayer(index).IsQ<T>();
        }

        public bool TryGetLayer<T>(int index, [NotNullWhen(true)] out T? result)
            where T : ILocationLayer
        {
            result = GetLayer<T>(index);

            return result.IsNotDefault();
        }

        public bool InBounds(Vector3Int pos) => Bounds.Contains(pos);

        private void SetLayers()
        {
            layers = GetComponentsInChildren<ILocationLayer>();
        }
    }
}
