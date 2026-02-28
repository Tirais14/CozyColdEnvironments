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
                                        LogType logType,
                                        DebugArguments args = DebugArguments.Default)
        {
            Guard.IsNotNull(message, nameof(message));

            switch (logType)
            {
                case LogType.None:
                    break;
                case LogType.Error:
                    source.PrintError(message, args);
                    break;
                case LogType.Warning:
                    source.PrintWarning(message, args);
                    break;
                case LogType.Log:
                    source.PrintLog(message, args);
                    break;
                case LogType.Exception:
                    source.PrintException(message.To<Exception>(), args);
                    break;
                default:
                    throw new InvalidOperationException(message.ToString());
            }

            return default!;
        }

        public static object PrintLog(this object? source,
                              object message,
                              DebugArguments args = DebugArguments.Default)
        {
            CCDebug.Instance.PrintLog(message, new DebugContext(source, args));
            return default!;
        }

        public static object PrintWarning(this object? source,
                                          object message,
                                          DebugArguments args = DebugArguments.Default)
        {
            CCDebug.Instance.PrintWarning(message, new DebugContext(source, args));
            return default!;
        }

        public static object PrintError(this object? source,
                                        object message,
                                        DebugArguments args = DebugArguments.Default)
        {
            CCDebug.Instance.PrintError(message, new DebugContext(source, args));
            return default!;
        }

        public static object PrintException(this object? source,
                                            Exception exception,
                                            DebugArguments args = DebugArguments.Default)
        {
            CCDebug.Instance.PrintException(exception, new DebugContext(source, args));
            return default!;
        }

        public static object PrintExceptionAsLog(this object? source,
                                                 Exception exception,
                                                 DebugArguments args = DebugArguments.Default)
        {
            CCDebug.Instance.PrintExceptionAsLog(exception, new DebugContext(source, args));
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
