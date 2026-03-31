using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public interface IBoxItem
    {
        MeshRenderer meshRenderer { get; }

        Collider collider { get; }

        Rigidbody? rigidbody { get; }

        Transform transform { get; }
    }

    public static class IBoxItemExtensions
    {
        public static Bounds GetBounds(this IBoxItem source)
        {
            CC.Guard.IsNotNullSource(source);

            if (!source.collider.enabled || source.collider.bounds.extents.sqrMagnitude.NearlyEquals(0f))
                return source.meshRenderer.bounds;

            return source.collider.bounds;
        }
    }
}
