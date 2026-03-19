using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class QuaternionExtensions
    {
        public static bool IsFinite(this Quaternion source)
        {
            return !float.IsNaN(source.x)
                   &&
                   !float.IsNaN(source.y)
                   &&
                   !float.IsNaN(source.z)
                   &&
                   !float.IsNaN(source.w);
        }
    }
}
