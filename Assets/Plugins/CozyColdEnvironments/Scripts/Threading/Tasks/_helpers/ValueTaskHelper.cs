using System;
using System.Threading.Tasks;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Threading.Tasks
{
    public static class ValueTaskHelper
    {
        //public static async ValueTask WhenAll(
        //    params ValueTask[] tasks
        //    )
        //{
        //    if (tasks.IsEmpty())
        //        return;

        //    var taskCount = tasks.Length;
        //    const int SKIP_FRAME_COUNT = 3;

        //    var completedTaskFlags = ArrayPool<bool>.Shared.Get(taskCount);

        //    try
        //    {
        //        for (int i = 0; i < taskCount * SKIP_FRAME_COUNT; i++)
        //        {
        //            if ((i % SKIP_FRAME_COUNT) != 0)
        //            {
        //                await Task.Yield();
        //                continue;
        //            }

        //            for (int j = 0; j < taskCount; j++)
        //            {
        //                if (completedTaskFlags[j])
        //                    continue;

        //                if (!tasks[j].IsCompleted)
        //                    continue;

        //                completedTaskFlags[j] = true;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        completedTaskFlags.Dispose();
        //    }
        //}

        //public static async ValueTask<T[]> WhenAll<T>(
        //    params ValueTask<T>[] tasks
        //    )
        //{
        //    if (tasks.IsEmpty())
        //        return Array.Empty<T>();

        //    var taskCount = tasks.Length;
        //    const int SKIP_FRAME_COUNT = 3;

        //    var results = new T[taskCount];

        //    var completedTaskFlags = ArrayPool<bool>.Shared.Get(taskCount);

        //    try
        //    {
        //        for (int i = 0; i < taskCount * SKIP_FRAME_COUNT; i++)
        //        {
        //            if ((i % SKIP_FRAME_COUNT) != 0)
        //            {
        //                await Task.Yield();
        //                continue;
        //            }

        //            for (int j = 0; j < taskCount; j++)
        //            {
        //                if (completedTaskFlags[j])
        //                    continue;

        //                if (!tasks[j].IsCompleted)
        //                    continue;

        //                results[j] = tasks[j].Result;
        //                completedTaskFlags[j] = true;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        completedTaskFlags.Dispose();
        //    }

        //    return results;
        //}

        public static async ValueTask DelayFrame(int frameCount)
        {
            if (frameCount < 1)
                return;

            for (int i = 0; i < frameCount; i++)
                await Task.Yield();
        }

        public static async void Forget(
            this ValueTask task,
            bool suppresCancellationThrow = true
            )
        {
            if (task.IsCompleted || task.IsFaulted)
                return;

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (suppresCancellationThrow && ex.IsCancellationException())
                    CCDebug.Instance.PrintLog(ex);
                else
                    CCDebug.Instance.PrintException(ex);
            }
        }

        public static async void Forget<TCaller>(
            this ValueTask task,
            TCaller? caller,
            bool suppresCancellationThrow = true
            )
        {
            if (task.IsCompleted || task.IsFaulted)
                return;

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (suppresCancellationThrow && ex.IsCancellationException())
                    caller.PrintLog(ex);
                else
                    caller.PrintException(ex);
            }
        }

        public static async void Forget<T>(
            this ValueTask<T> task,
            bool suppresCancellationThrow = true
            )
        {
            if (task.IsCompleted || task.IsFaulted)
                return;

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (suppresCancellationThrow && ex.IsCancellationException())
                    CCDebug.Instance.PrintLog(ex);
                else
                    CCDebug.Instance.PrintException(ex);
            }
        }

        public static async void Forget<T, TCaller>(
            this ValueTask<T> task,
            TCaller? caller, 
            bool suppresCancellationThrow = true
            )
        {
            if (task.IsCompleted || task.IsFaulted)
                return;

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (suppresCancellationThrow && ex.IsCancellationException())
                    caller.PrintLog(ex);
                else
                    caller.PrintException(ex);
            }
        }

        public static async void ForgetByPrintException<TCaller>(
            this ValueTask task,
            TCaller? caller,
            bool suppresCancellationThrow = true
            )
        {
            task.Forget(caller, suppresCancellationThrow);
        }

        public static async void ForgetByPrintException<T>(
            this ValueTask<T> task,
            bool suppresCancellationThrow = true
            )
        {
            task.Forget(suppresCancellationThrow);
        }

        public static async void ForgetByPrintException<T, TCaller>(
            this ValueTask<T> task,
            TCaller? caller,
            bool suppresCancellationThrow = true
            )
        {
            task.Forget(caller, suppresCancellationThrow);
        }
    }
}
