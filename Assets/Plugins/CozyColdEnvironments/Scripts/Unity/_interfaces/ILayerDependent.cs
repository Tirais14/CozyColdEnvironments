using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface ILayerDependent
    {
        int LayerMask { get; }
    }
}
