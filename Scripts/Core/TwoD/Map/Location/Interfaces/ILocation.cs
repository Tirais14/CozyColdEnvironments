#nullable enable
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UTIRLib.TwoD.Map
{
    public interface ILocation
    {
        BoundsInt Bounds { get; }
        int LayersCount { get; }
        ILocationLayer this[int index] { get; }

        ILocationLayer GetLayer(int index);

        T? GetLayer<T>(int index) where T : ILocationLayer;

        bool TryGetLayer<T>(int index, [NotNullWhen(true)] out T? result)
            where T : ILocationLayer;

        bool InBounds(Vector3Int pos);
    }
}
