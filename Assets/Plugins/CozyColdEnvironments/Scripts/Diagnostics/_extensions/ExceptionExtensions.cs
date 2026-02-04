using System;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public static class ExceptionExtensions
    {
        public static bool IsCancellationException(this Exception? source)
        {
            if (source is null)
                return false;

            return source is OperationCanceledException || source is TaskCanceledException;
        }
    }
}
