using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Humanizer;

#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;
#endif

#nullable enable
namespace CCEnvs.Async
{
    public sealed class AsyncTaskRegistry : IAsyncTaskRegistry
    {
#if UNITASK_PLUGIN
        private readonly List<UniTask> uniTasks = new();
#endif
        private readonly List<Task> tasks = new();
        private readonly List<ValueTask> valueTasks = new();
        private readonly Timer timer = new(100)
        {
            AutoReset = true
        };

        private int pendingTaskCount;
        private TimeSpan idleTimeAgo;
        private readonly Action onTaskCompleted;

        public int TaskCount {
            get
            {
                return tasks.Count
                    +
#if UNITASK_PLUGIN
                    uniTasks.Count
                    +
#endif
                    valueTasks.Count;
            }
        }

        public bool HasTasks {
            get
            {
                return tasks.Any(x => !x.IsCompleted)
                       ||
#if UNITASK_PLUGIN
                       uniTasks.Any(x => x.Status == UniTaskStatus.Pending)
                       ||
#endif
                       valueTasks.Any(x => !x.IsCompleted);
            }
        }

        public bool IsRunning { get; private set; }
        public TimeSpan IdleTimeBeforeDoneRunning { get; set; }

#if UNITASK_PLUGIN
        public void RegisterTask(UniTask task)
        {
            if (task.Status != UniTaskStatus.Pending)
                return;

            uniTasks.Add(task);
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
            TryStartTimer();
        }

        private void OnTimeElapsed(object _, ElapsedEventArgs __)
        {
            if (HasTasks)
            {
                idleTimeAgo = TimeSpan.Zero;
                return;
            }

            idleTimeAgo += 1.Seconds();

            if (idleTimeAgo < IdleTimeBeforeDoneRunning)
                return;

            timer.Stop();

#if UNITASK_PLUGIN
            uniTasks.Clear();
            uniTasks.TrimExcess();
#endif
            tasks.Clear();
            tasks.TrimExcess();

            valueTasks.Clear();
            valueTasks.TrimExcess();

            idleTimeAgo = TimeSpan.Zero;

            IsRunning = false;

            timer.Elapsed -= OnTimeElapsed;
        }

        private void TryStartTimer()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            timer.Start();
            timer.Elapsed += OnTimeElapsed;
        }
    }
}
