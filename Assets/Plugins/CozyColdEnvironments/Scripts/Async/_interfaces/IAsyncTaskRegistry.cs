using System;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Async
{
    public interface IAsyncTaskRegistry
    {
        int TaskCount { get; }
        bool HasTasks { get; }
        bool IsRunning { get; }
        TimeSpan IdleTimeBeforeDoneRunning { get; set; }

#if UNITASK_PLUGIN
        void RegisterTask(Cysharp.Threading.Tasks.UniTask task);
        void RegisterTask<T>(Cysharp.Threading.Tasks.UniTask<T> task);
#endif

        void RegisterTask(ValueTask task);
        void RegisterTask<T>(ValueTask<T> task);

        void RegisterTask(Task task);
    }
}
