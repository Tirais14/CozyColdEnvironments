using System;

#nullable enable
namespace CCEnvs
{
    public static class TirLibDebug 
    {
        public static IDebugLogger Logger { get; set; } = new DebugLogger();

        public static void PrintLog(object message,
                                    object? context = null)
        {
            Logger.PrintLog(message, context);
        }

        public static void PrintWarning(object message,
                                        object? context = null)
        {
            Logger.PrintWarning(message, context);
        }

        public static void PrintError(object message,
                                      object? context = null)
        {
            Logger.PrintError(message, context);
        }

        public static void PrintException(Exception exception,
                                          object? context = null)
        {
            Logger.PrintException(exception, context);
        }

        public static void AssertLog(bool condition,
                                     object message,
                                     object? context = null)
        {
            Logger.AssertLog(condition, message, context);
        }

        public static void AssertWarning(bool condition,
                                         object message,
                                         object? context = null)
        {
            Logger.AssertWarning(condition, message, context);
        }

        public static void AssertError(bool condition,
                                       object message,
                                       object? context = null)
        {
            Logger.AssertError(condition, message, context);
        }

        public static void AssertException(bool condition,
                                           Exception exception,
                                           object? context = null)
        {
            Logger.AssertException(condition, exception, context);
        }
    }
}
