#nullable enable
using CCEnvs.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Async
{
    public static class TaskHelper
    {
        public static async Task WaitWhile(Func<bool> func, CancellationToken cancellationToken = default)
        {
            Validate.ArgumentNull(func, nameof(func));

            while (func())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
    }
}
