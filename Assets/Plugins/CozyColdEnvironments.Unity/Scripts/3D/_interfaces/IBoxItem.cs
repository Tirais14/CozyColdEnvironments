using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public interface IBoxItem
    {
        MeshRenderer meshRenderer { get; }

        Rigidbody? rigidbody { get; }

        Collider collider { get; }

        Transform transform { get; }

        ExternalSize sizeInfo { get; }
    }
}
