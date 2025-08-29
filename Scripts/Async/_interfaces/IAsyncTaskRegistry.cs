using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

#nullable enable
namespace UTIRLib.Async
{
    public interface IAsyncTaskRegistry
    {
        event Action? OnTasksCompleted;

        int TaskCount { get; }
        bool HasTasks { get; }

        void RegisterTask(UniTask task);

        void RegisterTask(Task task);

        void RegisterTask(Func<bool> waitUntilFalse);

#if USE_ADDRESSABLES
        void RegisterTask(AsyncOperationHandle handle);
#endif
    }
}
