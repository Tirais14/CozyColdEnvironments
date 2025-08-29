using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

#nullable enable
namespace CozyColdEnvironments.Async
{
    public sealed class AsyncTaskRegistry : IAsyncTaskRegistry
    {
        private readonly List<UniTask> tasks = new();
        private readonly object lockObject = new();
        private bool isTaskTracking;

        public event Action? OnTasksCompleted;

        public int TaskCount => tasks.Count;
        public bool HasTasks => isTaskTracking || tasks.IsNotEmpty();

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

        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterTask(Func<bool> waitWhileFalse)
        {
            if (waitWhileFalse is null)
                throw new ArgumentNullException(nameof(waitWhileFalse));

            RegisterTask(UniTask.RunOnThreadPool(waitWhileFalse));
        }

#if USE_ADDRESSABLES
        public void RegisterTask(AsyncOperationHandle handle)
        {
            RegisterTask(handle.Task);
        }
#endif

        private void TrackTasks()
        {
            while (tasks.Count > 0)
            {
                UniTask task;
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
