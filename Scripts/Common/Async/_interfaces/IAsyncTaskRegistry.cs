#if UNI_TASK
using Cysharp.Threading.Tasks;
#endif
using System;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Async
{
    public interface IAsyncTaskRegistry
    {
        event Action? OnTasksCompleted;

        int TaskCount { get; }
        bool HasTasks { get; }

#if UNI_TASK
        void RegisterTask(UniTask task);
#endif

        void RegisterTask(Task task);

        void RegisterTask(Func<bool> waitUntilFalse);
    }
}
