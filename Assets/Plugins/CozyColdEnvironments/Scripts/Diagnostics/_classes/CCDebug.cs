using CCEnvs.Pools;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public sealed class CCDebug : IDebugLogger
    {
        public static IDebugLogger Instance { get; set; } = new CCDebug();

        internal static readonly Dictionary<Type, Action> onEnabledTypesChangedActions = new(0);

#if CC_DEBUG_ENABLED
        private static readonly HashSet<Type> disabledTypes = new();
#else
        private static readonly HashSet<Type> enabledTypes = new();
#endif

        public static bool IsEnabled {
            get => Instance.IsEnabled;
            set => Instance.IsEnabled = value;
        }

        bool IDebugLogger.IsEnabled { get; set; }
#if CC_DEBUG_ENABLED
         = true;
#endif

        public static bool IsTypeEnabled(Type type)
        {
            Guard.IsNotNull(type);

#if CC_DEBUG_ENABLED
            return Instance.IsEnabled && !disabledTypes.Contains(type);
#else
            return Instance.IsEnabled && enabledTypes.Contains(type);
#endif
        }

        public static bool IsTypeEnabled<T>()
        {
#if CC_DEBUG_ENABLED
            return Instance.IsEnabled && !disabledTypes.Contains(TypeofCache<T>.Type);
#else
            return Instance.IsEnabled && enabledTypes.Contains(TypeofCache<T>.Type);
#endif
        }

        public static void DisableType(Type type)
        {
            Guard.IsNotNull(type);

#if CC_DEBUG_ENABLED
            disabledTypes.Add(type);
#else
            enabledTypes.Remove(type);
#endif

            OnEnabledTypeChanged(type);
        }

        public static void DisableTypes(params Type[] types)
        {
            Guard.IsNotNull(types);

            for (int i = 0; i < types.Length; i++)
                disabledTypes.Remove(types[i]);
        }

        public static void EnableType(Type type)
        {
            Guard.IsNotNull(type);

#if CC_DEBUG_ENABLED
            disabledTypes.Remove(type);
#else
            enabledTypes.Add(type);
#endif

            OnEnabledTypeChanged(type);
        }

        public static void EnableTypes(params Type[] types)
        {
            Guard.IsNotNull(types);

            for (int i = 0; i < types.Length; i++)
                EnableType(types[i]);
        }

        public static void PrintDebug(object message, LogType logType, object? context = null)
        {
            context.PrintDebug(message, logType);
        }

        public static void PrintLog(object message, object? context = null)
        {
            Instance.PrintLog(message, context);
        }

        public static void PrintWarning(object message, object? context = null)
        {
            Instance.PrintWarning(message, context);
        }

        public static void PrintError(object message, object? context = null)
        {
            Instance.PrintError(message, context);
        }

        public static void PrinException(Exception exception, object? context = null)
        {
            Instance.PrintException(exception, context);
        }

        public static void AssertLog(bool condition, object message, object? context = null)
        {
            Instance.AssertLog(condition, message, context);
        }

        public static void AssertWarning(bool condition, object message, object? context = null)
        {
            Instance.AssertWarning(condition, message, context);
        }

        public static void AssertError(bool condition, object message, object? context = null)
        {
            Instance.AssertError(condition, message, context);
        }

        public static void AssertException(bool condition, Exception exception, object? context = null)
        {
            Instance.AssertException(condition, exception, context);
        }

        private static void WriteTypeName(object target, StringBuilder stringBuilder)
        {
            if (target is not Type type)
                type = target.GetType();

            string typeName = type.Name;

            if (type.IsGenericType)
            {
                for (int i = 0; i < typeName.Length - 2; i++)
                    stringBuilder.Append(typeName[i]);

                WriteGenericArguments(type, stringBuilder);
            }
            else
                stringBuilder.Append(typeName);

            static void WriteGenericArguments(Type type, StringBuilder stringBuilder)
            {
                Type[] genericArguments = type.GetGenericArguments();

                using var genericArgumentNames = new PooledList<Type>();

                stringBuilder.Append('<');

                Type genericArgument;

                for (int i = 0; i < genericArguments.Length; i++)
                {
                    genericArgument = genericArguments[i];

                    WriteTypeName(genericArgument, stringBuilder);
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append('>');
            }
        }

        private static string GetMessage(object target, object? context)
        {
            using var stringBuilder = StringBuilderPool.Shared.Get();

            var targetString = target.ToString();

            if (context is not null)
            {
                WriteContextInfo(context, stringBuilder.Value);
                stringBuilder.Value.Append(": ");
                WriteTargetInfo(targetString, stringBuilder.Value);
            }
            else
                WriteTargetInfo(targetString, stringBuilder.Value);

            return stringBuilder.Value.ToString();
        }

        private static void WriteContextInfo(object context, StringBuilder stringBuilder)
        {
            object? contextTarget = context;

            if (context is DebugContext debugContext)
                contextTarget = debugContext.Target;

            if (contextTarget is null)
                return;

            if (context is Object unityObj)
            {
                WriteTypeName(unityObj, stringBuilder);

                stringBuilder.Append('(');
                stringBuilder.Append(unityObj.name);
                stringBuilder.Append(')');
            }
            else
                WriteTypeName(contextTarget, stringBuilder);
        }

        private static void WriteTargetInfo(object target, StringBuilder stringBuilder)
        {
            string targetString = target.ToString();

            if (targetString.EndsWith('.'))
                for (int i = 0; i < targetString.Length - 1; i++)
                    stringBuilder.Append(targetString[i]);
            else
                stringBuilder.Append(targetString);
        }

        private static void OnEnabledTypeChanged(Type type)
        {
            if (onEnabledTypesChangedActions.TryGetValue(type, out Action onEnabledTypeChangedAction))
                onEnabledTypeChangedAction();
        }

        void IDebugLogger.PrintLog(object message, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        void IDebugLogger.PrintWarning(object message, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        void IDebugLogger.PrintError(object message, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogError(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        void IDebugLogger.PrintExceptionAsLog(Exception exception, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log($"{exception.GetType().Name}: {exception.Message}", context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        void IDebugLogger.PrintExceptionAsWarning(Exception exception, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning($"{exception.GetType().Name}: {exception.Message}", context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        void IDebugLogger.PrintException(Exception exception, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        void IDebugLogger.AssertLog(bool condition, object message, object? context)
        {
            if (condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.Log(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        void IDebugLogger.AssertWarning(bool condition, object message, object? context)
        {
            if (condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        void IDebugLogger.AssertError(bool condition, object message, object? context)
        {
            if (condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogError(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        void IDebugLogger.AssertException(bool condition, Exception exception, object? context)
        {
            if (condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception);
#endif
        }
    }

    public static class CCDebug<T>
    {
        private static bool? isEnabled;

        public static bool IsEnabled {
            get
            {
                isEnabled ??= CCDebug.IsTypeEnabled(typeof(T));
                return isEnabled.Value;
            }
            set => CCDebug.EnableType(typeof(T));
        }

        static CCDebug()
        {
            CCDebug.onEnabledTypesChangedActions.Add(typeof(T), () => isEnabled = null);
        }
    }
}
