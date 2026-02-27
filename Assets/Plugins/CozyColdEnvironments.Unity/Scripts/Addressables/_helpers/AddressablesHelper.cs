#if ADDRESSABLES_PLUGIN
using UnityEngine;
using UnityEngine.AddressableAssets;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressablesHelper
    {
        public static void ReleasePrefabComponentAsset<T>(T cmp)
        {
            CC.Guard.IsNotNull(cmp, nameof(cmp));

            Addressables.Release(cmp.To<Component>().gameObject);
        }

        //public static void IsAssetLoaded(object asset)
        //{
        //    AsyncOperationHandle<Component> handle = Addressables.LoadAssetAsync<Component>();
        //}
    }
}
#endif