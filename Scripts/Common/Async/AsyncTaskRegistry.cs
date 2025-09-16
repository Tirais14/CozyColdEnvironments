#if UNI_TASK
using Cysharp.Threading.Tasks;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Async
{
    public sealed class AsyncTaskRegistry : IAsyncTaskRegistry
    {
#if UNI_TASK
        private readonly List<UniTask> tasks = new();
#else
        private readonly List<Task> tasks = new();
#endif

        private readonly object lockObject = new();
        private bool isTaskTracking;

        public event Action? OnTasksCompleted;

        public int TaskCount => tasks.Count;
        public bool HasTasks => isTaskTracking || tasks.Any();

#if UNI_TASK
        /// <exception cref="ArgumentException"></exception>
        public void RegisterTask(UniTask task)
        {
            if (task.Equals(default(UniTask)))
                throw new ArgumentException(nameof(task));

            tasks.Add(task);

            if (!isTaskTracking)
            {
                UniTask.RunOnThreadPool(TrackTasks).Forget();
                isTaskTracking = true;
            }
        }
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterTask(Task task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            RegisterTask(task.AsUniTask());
        }
#else
        /// <exception cref="ArgumentException"></exception>
        public void RegisterTask(Task task)
        {
            if (task.Equals(default(Task)))
                throw new ArgumentException(nameof(task));

            tasks.Add(task);

            if (!isTaskTracking)
            {
                _ = Task.Run(TrackTasks);
                isTaskTracking = true;
            }
        }
#endif

        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterTask(Func<bool> waitUntilFalse)
        {
            if (waitUntilFalse is null)
                throw new ArgumentNullException(nameof(waitUntilFalse));

#if UNI_TASK
            RegisterTask(UniTask.RunOnThreadPool(waitUntilFalse));
#else
            RegisterTask(Task.Run(waitUntilFalse));
#endif
        }

        private void TrackTasks()
        {
            while (tasks.Count > 0)
            {
#if UNI_TASK
                UniTask task;
#else
                Task task;
#endif
                int count = tasks.Count;
                for (int i = 0; i < count; i++)
                {
                    task = tasks[i];
                    if (task.Status == UniTaskStatus.Succeeded ||
                        task.Status == UniTaskStatus.Faulted ||
                        task.Status == UniTaskStatus.Canceled
                        )
                    {
                        tasks.RemoveAt(i);
                        count--;
                    }
                }
            }

            lock (lockObject)
            {
                isTaskTracking = false;
            }

            OnTasksCompleted?.Invoke();
        }
    }
}
