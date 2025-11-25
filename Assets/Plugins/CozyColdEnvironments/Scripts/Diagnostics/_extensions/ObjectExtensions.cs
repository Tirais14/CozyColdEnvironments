#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#pragma warning disable S3236
namespace CCEnvs.Diagnostics
{
    public static class ObjectExtensions
    {
        public static DebugContext AsDebugContext(this object? value,
                                  DebugArguments arguments = default)
        {
            return new DebugContext(value, arguments);
        }

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
                    source.PrintException(message.As<Exception>(), args);
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
            CCDebug.PrintLog(message, new DebugContext(source, args));

            return default!;
        }

        public static object PrintWarning(this object? source,
                                          object message,
                                          DebugArguments args = DebugArguments.Default)
        {
            CCDebug.PrintWarning(message, new DebugContext(source, args));

            return default!;
        }

        public static object PrintError(this object? source,
                                        object message,
                                        DebugArguments args = DebugArguments.Default)
        {
            CCDebug.PrintError(message, new DebugContext(source, args));

            return default!;
        }

        public static object PrintException(this object? source,
                                            Exception exception,
                                            DebugArguments args = DebugArguments.Default)
        {
            CCDebug.PrintException(exception, new DebugContext(source, args));

            return default!;
        }

        public static object PrintExceptionAsLog(this object? source,
                                                 Exception exception,
                                                 DebugArguments args = DebugArguments.Default)
        {
            CCDebug.PrintExceptionAsLog(exception, new DebugContext(source, args));

            return default!;
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
