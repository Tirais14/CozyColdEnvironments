#nullable enable
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CCEnvs.Diagnostics
{
    public static class ObjectExtensions
    {
        public static Result PrintLog(this object? source,
                                      object message,
                                      DebugArguments args = DebugArguments.Default) 
        {
            CC.Validate.ArgumentNull(message, nameof(message));

            CCDebug.PrintLog(message, new DebugContext(source, args));

            return default;
        }

        public static Result PrintWarning(this object? source,
                                          object message,
                                          DebugArguments args = DebugArguments.Default)
        {
            CC.Validate.ArgumentNull(message, nameof(message));

            CCDebug.PrintWarning(message, new DebugContext(source, args));

            return default;
        }

        public static Result PrintError(this object? source,
                                        object message,
                                        DebugArguments args = DebugArguments.Default)
        {
            CC.Validate.ArgumentNull(message, nameof(message));

            CCDebug.PrintError(message, new DebugContext(source, args));

            return default;
        }

        public static Result PrintException(this object? source,
                                            Exception exception,
                                            DebugArguments args = DebugArguments.Default)
        {
            CC.Validate.ArgumentNull(exception, nameof(exception));

            CCDebug.PrintException(exception, new DebugContext(source, args));

            return default;
        }

        public static Result PrintExceptionAsLog(this object? source,
                                                 Exception exception,
                                                 DebugArguments args = DebugArguments.Default)
        {
            CC.Validate.ArgumentNull(exception, nameof(exception));

            CCDebug.PrintExceptionAsLog(exception, new DebugContext(source, args));

            return default;
        }

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj)
        {
            return new NullValidator<T>(obj).IsNull;
        }

        /// <summary>Checks for unity or system <see langword="null"/></summary>
        public static bool IsNull<T>([NotNullWhen(false)] this T? obj, out NullValidator<T> validationResult)
        {
            validationResult = new NullValidator<T>(obj);

            return validationResult.IsNull;
        }

        /// <summary>Inverted</summary>
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? obj)
        {
            return !new NullValidator<T>(obj).IsNull;
        }
    }
}
