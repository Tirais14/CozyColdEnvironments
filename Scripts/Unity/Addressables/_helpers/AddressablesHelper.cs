using CCEnvs.Common;
using UnityEngine.AddressableAssets;

#nullable enable
namespace CCEnvs.Unity.AddressableAssets
{
    public static class AddressablesHelper
    {
        public static T LoadAsset<T>(object key)
            where T : UnityEngine.Object
        {
            try
            {
                var task = Addressables.LoadAssetAsync<T>(key).Task;
                task.RunSynchronously();

                return task.Result;
            }
            catch (System.Exception ex)
            {
                CCDebug.PrintException(ex);
                throw;
            }
        }
    }
}
