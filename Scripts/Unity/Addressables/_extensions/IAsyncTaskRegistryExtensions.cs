using CCEnvs.Async;
using UnityEngine.ResourceManagement.AsyncOperations;

#nullable enable
namespace CCEnvs.Unity.AddressableAssets
{
    public static class IAsyncTaskRegistryExtensions
    {
        public static void RegisterTask(this IAsyncTaskRegistry value,
                                        AsyncOperationHandle handle)
        {
            value.RegisterTask(handle.Task);
        }
    }
}
