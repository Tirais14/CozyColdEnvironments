#nullable enable
using System;

namespace CCEnvs
{
    public interface IDebugLogger : IToggleable
    {
        void EnableDebugFor(params Type[] types);

        void DisableDebugFor(params Type[] types);

        void PrintLog(object message, object? context = null);

        void PrintWarning(object message, object? context = null);

        void PrintError(object message, object? context = null);

        void PrintException(Exception exception, object? context = null);

        void AssertLog(bool condition, object message, object? context);

        void AssertWarning(bool condition, object message, object? context);

        void AssertError(bool condition, object message, object? context);

        void AssertException(bool condition, Exception exception, object? context);
    }
}
