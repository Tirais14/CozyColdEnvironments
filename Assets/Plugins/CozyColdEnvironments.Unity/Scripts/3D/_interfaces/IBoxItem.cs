using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public interface IBoxItem
    {
        Bounds bounds { get; }

        Collider collider { get; }

        Rigidbody? rigidbody { get; }

        Transform transform { get; }
    }
}
