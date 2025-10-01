#if UNI_TASK
using Cysharp.Threading.Tasks;
#endif
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Async
{
    public interface IAsyncTaskRegistry
    {
        int TaskCount { get; }
        bool HasTasks { get; }
        bool IsRunning { get; }

#if UNI_TASK
        void RegisterTask(UniTask task);
#endif

        void RegisterTask(ValueTask task);

        void RegisterTask(Task task);
    }
}
