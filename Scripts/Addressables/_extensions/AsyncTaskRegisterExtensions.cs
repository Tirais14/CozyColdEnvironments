using UnityEngine.ResourceManagement.AsyncOperations;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.AddressableAssets
{
    public static class AsyncTaskRegisterExtensions
    {
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void RegisterTask(this IAsyncTaskRegistry value,
                                        AsyncOperationHandle handle)
        {
            if (value.IsNull())
                throw new System.ArgumentNullException("value");

            value.RegisterTask(handle.Task);
        }
    }
}
