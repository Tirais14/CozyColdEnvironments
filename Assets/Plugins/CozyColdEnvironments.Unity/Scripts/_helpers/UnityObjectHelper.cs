using CCEnvs.TypeMatching;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public static class UnityObjectHelper
    {
        public static bool DestroyByGameObject(this Object source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.Is<GameObject>(out var go))
            {
                Object.Destroy(go);
                return true;
            }
            else if (source.Is<Component>(out var cmp))
            {
                Object.Destroy(cmp.gameObject);
                return true;
            }

            return false;
        }
    }
}
