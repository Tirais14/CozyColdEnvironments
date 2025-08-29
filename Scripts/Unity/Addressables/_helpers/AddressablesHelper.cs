using UnityEngine;
using UnityEngine.AddressableAssets;

#nullable enable
namespace CozyColdEnvironments.AddressableAssets
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
                TirLibDebug.PrintException(ex);
                throw;
            }
        }
    }
}
