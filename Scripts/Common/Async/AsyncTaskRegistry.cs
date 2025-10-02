#if UNI_TASK
using Cysharp.Threading.Tasks;

#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Timers;

#nullable enable
namespace CCEnvs.Async
{
    public sealed class AsyncTaskRegistry : IAsyncTaskRegistry
    {
#if UNI_TASK
        private readonly List<UniTask> uniTasks = new();
#endif
        private readonly List<Task> tasks = new();
        private readonly List<ValueTask> valueTasks = new();
        private readonly Timer timer = new(1000)
        {
            AutoReset = true
        };
        private int timerTriggeredTimes;

#if UNI_TASK
        public int TaskCount => tasks.Count + uniTasks.Count + valueTasks.Count;
        public bool HasTasks => tasks.Any() || uniTasks.Any() || valueTasks.Any();
#else
        public int TaskCount => tasks.Count + valueTasks.Count;
        public bool HasTasks => tasks.Any() || valueTasks.Any();
#endif
        public bool IsRunning => timer.Enabled && HasTasks;

#if UNI_TASK
        public void RegisterTask(UniTask task)
        {
            if (task.Status == UniTaskStatus.Succeeded
                ||
                task.Status == UniTaskStatus.Faulted
                ||
                task.Status == UniTaskStatus.Canceled
                )
                return;

            uniTasks.Add(task);
            task.ToObservable().Subscribe(_ => uniTasks.Remove(task));
            TryStartTimer();
        }
        public void RegisterTask<T>(UniTask<T> task)
        {
            RegisterTask((UniTask)task);
        }
#endif

        public void RegisterTask(ValueTask task)
        {
            if (task.IsCompleted || task.IsFaulted || task.IsCanceled)
                return;

            valueTasks.Add(task);
            task.AsTask().ToObservable().Subscribe(_ => valueTasks.Remove(task));
            TryStartTimer();
        }
        public void RegisterTask<T>(ValueTask<T> task)
        {
            RegisterTask(task.AsTask());
        }

        public void RegisterTask(Task task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));
            if (task.IsFaulted || task.IsCompleted || task.IsCanceled)
                return;

            tasks.Add(task);
            task.ToObservable().Subscribe(_ => tasks.Remove(task));
            TryStartTimer();
        }

        private void TryStartTimer()
        {
            if (IsRunning)
                return;

            timer.Start();
            timer.Elapsed += (_, _) =>
            {
                if (HasTasks)
                    return;

                timerTriggeredTimes++;

                if (timerTriggeredTimes > 1)
                {
                    timer.Stop();
                    timerTriggeredTimes = 0;
                }
            };
        }
    }
}
