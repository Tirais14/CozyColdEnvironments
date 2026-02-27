using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public sealed class CCDebug : IDebugLogger
    {
        public static IDebugLogger Instance { get; private set; } = new CCDebug();

        private readonly HashSet<Type> disabled = new(0);
        private readonly HashSet<Type> disabledAdditive = new(0);

        public bool IsEnabled { get; set; }
#if CC_DEBUG_ENABLED
         = true;
#endif

        public static void SetLogger(IDebugLogger logger)
        {
            CC.Guard.IsNotNull(logger, nameof(logger));
            Instance = logger;
        }

        public void DisableDebugFor(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
                disabled.Add(types[i]);
        }

        public void EnableDebugFor(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
                disabled.Remove(types[i]);
        }

        public void PrintLog(object message, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.Log(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void PrintWarning(object message, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void PrintError(object message, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogError(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void PrintExceptionAsLog(Exception exception, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.Log($"{exception.GetType().Name}: {exception.Message}", context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        public void PrintExceptionAsWarning(Exception exception, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning($"{exception.GetType().Name}: {exception.Message}", context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        public void PrintException(Exception exception, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception.Message);
#endif
        }

        public void AssertLog(bool condition, object message, object? context)
        {
            if (!IsPrintAllowed(context) || condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.Log(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void AssertWarning(bool condition, object message, object? context)
        {
            if (!IsPrintAllowed(context) || condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void AssertError(bool condition, object message, object? context)
        {
            if (!IsPrintAllowed(context) || condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogError(GetMessage(message, context), context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void AssertException(bool condition, Exception exception, object? context)
        {
            if (!IsPrintAllowed(context) || condition)
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(exception);
#endif
        }

        private static string GetTypeName(object target)
        {
            if (target is not Type type)
                type = target.GetType();

            string typeName = type.Name;
            if (type.IsGenericType)
            {
                typeName = typeName[..^2];
                typeName += ConvertGenericArguments();
            }

            return typeName;

            string ConvertGenericArguments()
            {
                var sb = new StringBuilder();

                sb.Append('<');

                IEnumerable<string> names =
                    from type in type.GetGenericArguments()
                    select GetTypeName(type) into name
                    select name;

                sb.AppendJoin(", ", names);

                sb.Append('>');

                return sb.ToString();
            }
        }

        private static string GetMessage(object target, object? context)
        {
            target = (target?.ToString() ?? string.Empty).TrimEnd('.');

            if (context is not null)
                return $"{GetContextInfo(context)}: {GetTargetInfo(target)}.";

            return $"{target}.";
        }

        private static string GetContextInfo(object context)
        {
            object? contextTarget = context;

            if (context is DebugContext debugContext)
                contextTarget = debugContext.Target;

            if (contextTarget is null)
                return string.Empty;

            if (context is Object unityObj)
                return $"{GetTypeName(unityObj)}({unityObj.name})";

            return $"{GetTypeName(contextTarget)}";
        }

        private static string GetTargetInfo(object target)
        {
            return (target?.ToString() ?? string.Empty).TrimEnd('.');
        }

        private bool IsPrintAllowed(object? context)
        {
            if (!IsEnabled)
                return false;

            object? target;
            DebugArguments debugArguments = default;
            if (context is DebugContext debugContext)
                debugContext.Deconstruct(out target, out debugArguments);
            else
                target = context;

            if (target is not null)
            {
                if (target is not Type contextType)
                    contextType = target.GetType();

                if (disabled.Contains(contextType))
                    return false;
                if (debugArguments.IsFlagSetted(DebugArguments.IsAdditive)
                    &&
                    disabledAdditive.Contains(contextType)
                    )
                    return false;
            }

            return true;
        }
    }
}
