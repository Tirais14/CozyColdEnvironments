using System;
using System.Buffers;
using System.Threading.Tasks;
using CCEnvs.Collections;

#nullable enable
namespace CCEnvs
{
    public static class ValueTaskHelper
    {
        public static async ValueTask WhenAll(
            params ValueTask[] tasks
            )
        {
            if (tasks.IsEmpty())
                return;

            var taskCount = tasks.Length;
            const int SKIP_FRAME_COUNT = 3;

            var completedTaskFlags = ArrayPool<bool>.Shared.Get(taskCount);

            try
            {
                for (int i = 0; i < taskCount * SKIP_FRAME_COUNT; i++)
                {
                    if ((i % SKIP_FRAME_COUNT) != 0)
                    {
                        await Task.Yield();
                        continue;
                    }

                    for (int j = 0; j < taskCount; j++)
                    {
                        if (completedTaskFlags[j])
                            continue;

                        if (!tasks[j].IsCompleted)
                            continue;

                        completedTaskFlags[j] = true;
                    }
                }
            }
            finally
            {
                completedTaskFlags.Dispose();
            }
        }

        public static async ValueTask<T[]> WhenAll<T>(
            params ValueTask<T>[] tasks
            )
        {
            if (tasks.IsEmpty())
                return Array.Empty<T>();

            var taskCount = tasks.Length;
            const int SKIP_FRAME_COUNT = 3;

            var results = new T[taskCount];

            var completedTaskFlags = ArrayPool<bool>.Shared.Get(taskCount);

            try
            {
                for (int i = 0; i < taskCount * SKIP_FRAME_COUNT; i++)
                {
                    if ((i % SKIP_FRAME_COUNT) != 0)
                    {
                        await Task.Yield();
                        continue;
                    }

                    for (int j = 0; j < taskCount; j++)
                    {
                        if (completedTaskFlags[j])
                            continue;

                        if (!tasks[j].IsCompleted)
                            continue;

                        results[j] = tasks[j].Result;
                        completedTaskFlags[j] = true;
                    }
                }
            }
            finally
            {
                completedTaskFlags.Dispose();
            }

            return results;
        }

        public static async ValueTask DelayFrame(int frameCount)
        {
            if (frameCount < 1)
                return;

            for (int i = 0; i < frameCount; i++)
                await Task.Yield();
        }
    }
}
