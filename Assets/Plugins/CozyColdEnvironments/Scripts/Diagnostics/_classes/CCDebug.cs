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

        private static readonly HashSet<Type> enabledTypes = new();

        public bool IsEnabled { get; set; }
#if CC_DEBUG_ENABLED
         = true;
#endif

        public static void SetLogger(IDebugLogger logger)
        {
            CC.Guard.IsNotNull(logger, nameof(logger));
            Instance = logger;
        }

        public static bool IsTypeEnabled(Type type)
        {
            Guard.IsNotNull(type);

            return Instance.IsEnabled && enabledTypes.Contains(type);
        }

        public static bool IsTypeEnabled<T>()
        {
            return Instance.IsEnabled && enabledTypes.Contains(TypeofCache<T>.Type);
        }

        public static void DisableTypes(params Type[] types)
        {
            Guard.IsNotNull(types);

            for (int i = 0; i < types.Length; i++)
                enabledTypes.Remove(types[i]);
        }

        public static void EnableTypes(params Type[] types)
        {
            Guard.IsNotNull(types);

            for (int i = 0; i < types.Length; i++)
                enabledTypes.Add(types[i]);
        }

        public void PrintLog(object message, object? context = null)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void PrintWarning(object message, object? context = null)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void PrintError(object message, object? context = null)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogError(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void PrintExceptionAsLog(Exception exception, object? context = null)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log($"{exception.GetType().Name}: {exception.Message}", context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        public void PrintExceptionAsWarning(Exception exception, object? context = null)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning($"{exception.GetType().Name}: {exception.Message}", context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        public void PrintException(Exception exception, object? context = null)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        public void AssertLog(bool condition, object message, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void AssertWarning(bool condition, object message, object? context)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void AssertError(bool condition, object message, object? context)
        {
            if (condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogError(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void AssertException(bool condition, Exception exception, object? context)
        {
            if (condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception);
#endif
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
    }
}
