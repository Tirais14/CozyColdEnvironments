using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using LogType = CCEnvs.Diagnostics.LogType;

#nullable enable
namespace CCEnvs
{
    public static class CCDebugExtensions
    {
        public static object PrintDebug(this object? source,
                                        object message,
                                        LogType logType)
        {
            Guard.IsNotNull(message, nameof(message));

            switch (logType)
            {
                case LogType.None:
                    break;
                case LogType.Error:
                    source.PrintError(message);
                    break;
                case LogType.Warning:
                    source.PrintWarning(message);
                    break;
                case LogType.Log:
                    source.PrintLog(message);
                    break;
                case LogType.Exception:
                    source.PrintException(message.CastTo<Exception>());
                    break;
                default:
                    throw new InvalidOperationException(message.ToString());
            }

            return default!;
        }

        public static object PrintLog(this object? source,
                              object message)
        {
            CCDebug.Instance.PrintLog(message, source);
            return default!;
        }

        public static object PrintWarning(this object? source,
                                          object message)
        {
            CCDebug.Instance.PrintWarning(message, source);
            return default!;
        }

        public static object PrintError(this object? source,
                                        object message)
        {
            CCDebug.Instance.PrintError(message, source);
            return default!;
        }

        public static object PrintException(this object? source,
                                            Exception exception)
        {
            CCDebug.Instance.PrintException(exception, source);
            return default!;
        }

        public static object PrintExceptionAsLog(this object? source,
                                                 Exception exception)
        {
            CCDebug.Instance.PrintExceptionAsLog(exception, source);
            return default!;
        }

        public static object AssertLog(this object? source, bool condition, object message)
        {
            CCDebug.Instance.AssertLog(condition, message, source);
            return default!;
        }

        public static object AssertWarning(this object? source, bool condition, object message)
        {
            CCDebug.Instance.AssertWarning(condition, message, source);
            return default!;
        }

        public static object AssertError(this object? source, bool condition, object message)
        {
            CCDebug.Instance.AssertError(condition, message, source);
            return default!;
        }
    }
}
