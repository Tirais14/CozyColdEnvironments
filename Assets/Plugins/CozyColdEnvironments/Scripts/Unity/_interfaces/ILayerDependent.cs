using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface ILayerDependent
    {
        LayerMask LayerMask { get; }
    }
}
