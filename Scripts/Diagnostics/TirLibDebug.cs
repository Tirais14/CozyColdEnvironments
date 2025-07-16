using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Collections;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.Diagnostics
{
    [SuppressMessage("Minor Bug", "S3887:Mutable, non-private fields should not be \"readonly\"", Justification = "<Pending>")]
    public static class TirLibDebug
    {
        public static readonly Flags<Type> debugTypes = new(inverted: true);

        public static readonly Flags<LogType> logTypes = new(){
            LogType.Log,
            LogType.Warning,
            LogType.Error
        };

#if UNITY_EDITOR
        /// <summary>
        /// !Only in editor
        /// </summary>
        public static bool DrawEnabled { get; set; } = true;

        public static bool Enabled { get; set; } = true;
        public static bool ExtraInfoEnabled { get; set; } = true;
#else
        public static bool Enabled { get; set; }
        public static bool ExtraInfoEnabled { get; set; } = false;
#endif

        public static void Log(string message, bool isExtraInfo = false)
        {
            LogInternal(LogType.Log, message, null, assertCondition: null, isExtraInfo);
        }

        public static void Log(string message, object context, bool isExtraInfo = false)
        {
            LogInternal(LogType.Log, message, context, assertCondition: null, isExtraInfo);
        }

        public static void LogFormat(string message, params object[] args)
        {
            LogInternal(LogType.Log, message, null, assertCondition: null, isExtraInfo: false, args);
        }

        public static void LogFormat(string message, object context, params object[] args)
        {
            LogInternal(LogType.Log, message, context, assertCondition: null, isExtraInfo: false, args);
        }

        public static void LogFormat(string message, bool isExtraInfo, params object[] args)
        {
            LogInternal(LogType.Log, message, null, assertCondition: null, isExtraInfo, args);
        }

        public static void LogFormat(string message,
                                     object context,
                                     bool isExtraInfo,
                                     params object[] args)
        {
            LogInternal(LogType.Log, message, context, assertCondition: null, isExtraInfo, args);
        }

        public static void Warning(string message, bool isExtraInfo = false)
        {
            LogInternal(LogType.Warning, message, null, assertCondition: null, isExtraInfo);
        }

        public static void Warning(string message, object context, bool isExtraInfo = false)
        {
            LogInternal(LogType.Warning, message, context, assertCondition: null, isExtraInfo);
        }

        public static void WarningFormat(string message, params object[] args)
        {
            LogInternal(LogType.Warning, message, null, assertCondition: null, isExtraInfo: false, args);
        }

        public static void WarningFormat(string message, object context, params object[] args)
        {
            LogInternal(LogType.Warning, message, context, assertCondition: null, isExtraInfo: false, args);
        }

        public static void WarningFormat(string message, bool isExtraInfo, params object[] args)
        {
            LogInternal(LogType.Warning, message, null, assertCondition: null, isExtraInfo, args);
        }

        public static void WarningFormat(string message,
                                         bool isExtraInfo,
                                         object context,
                                         params object[] args)
        {
            LogInternal(LogType.Warning, message, context, assertCondition: null, isExtraInfo, args);
        }

        public static void Error(string message, bool isExtraInfo = false)
        {
            LogInternal(LogType.Error, message, null, assertCondition: null, isExtraInfo);
        }

        public static void Error(string message, object context, bool isExtraInfo = false)
        {
            LogInternal(LogType.Error, message, context, assertCondition: null, isExtraInfo);
        }
        public static void Error(Exception exception, object context)
        {
            string message = exception.ToString();
            message = message[(exception.GetTypeName().Length + 1)..];

            LogInternal(LogType.Error, message, context, assertCondition: false, isExtraInfo: false);
        }
        public static void Error(Exception exception)
        {
            Error(exception, context: null!);
        }

        public static void ErrorFormat(string message, params object[] args)
        {
            LogInternal(LogType.Error, message, null, assertCondition: null, isExtraInfo: false, args);
        }

        public static void ErrorFormat(string message, object context, params object[] args)
        {
            LogInternal(LogType.Error, message, context, assertCondition: null, isExtraInfo: false, args);
        }

        public static void ErrorFormat(string message, bool isExtraInfo, params object[] args)
        {
            LogInternal(LogType.Error, message, null, assertCondition: null, isExtraInfo, args);
        }

        public static void ErrorFormat(string message,
                                       bool isExtraInfo,
                                       object context,
                                       params object[] args)
        {
            LogInternal(LogType.Error, message, context, assertCondition: null, isExtraInfo, args);
        }

        public static void Assert(bool condition, string message)
        {
            LogInternal(LogType.Error, message, null, condition, isExtraInfo: false);
        }

        public static void Assert(bool condition, string message, object context)
        {
            LogInternal(LogType.Error, message, context, condition, isExtraInfo: false);
        }

        public static void AssertFormat(bool condition, string message, params object[] args)
        {
            LogInternal(LogType.Error, message, null, condition, isExtraInfo: false, args);
        }

        public static void AssertFormat(bool condition,
                                        string message,
                                        object context,
                                        params object[] args)
        {
            LogInternal(LogType.Error, message, context, condition, isExtraInfo: false, args);
        }

        public static void AssertWarning(bool condition, string message)
        {
            LogInternal(LogType.Warning, message, null, condition, isExtraInfo: false);
        }

        public static void AssertWarning(bool condition, string message, object context)
        {
            LogInternal(LogType.Warning, message, context, condition, isExtraInfo: false);
        }

        public static void AssertWarningFormat(bool condition, string message, params object[] args)
        {
            LogInternal(LogType.Warning, message, null, condition, isExtraInfo: false, args);
        }

        public static void AssertWarningFormat(bool condition,
                                               string message,
                                               object context,
                                               params object[] args)
        {
            LogInternal(LogType.Warning, message, context, condition, isExtraInfo: false, args);
        }

        private static void LogInternal(LogType logType,
                                        string message,
                                        object? context,
                                        bool? assertCondition,
                                        bool isExtraInfo,
                                        params object[] args)
        {
            if (!Enabled
                ||
                assertCondition.HasValue && !assertCondition.Value
                ||
                !logTypes.Contains(logType)
                ||
                isExtraInfo && !ExtraInfoEnabled
                ||
                IsDisabledType(context)
                )
                return;

            message = GetContextMessage(context) + message;

            if (args.IsNotEmpty())
                message = string.Format(message, args);

            if (context is Object unityContext)
                Debug.unityLogger.Log(logType, message, context: unityContext);
            else
                Debug.unityLogger.Log(logType, message);
        }

        private static bool IsDisabledType(object? context)
        {
            if (context.IsNotNull()) return !debugTypes.Contains(context.GetType());

            return false;
        }

        private static string GetContextMessage(object? context)
        {
            if (context.IsNotNull()) return nameof(UTIRLib) + '.' + context.GetTypeName() + ": ";
            else return nameof(UTIRLib) + ": ";
        }
    }
}