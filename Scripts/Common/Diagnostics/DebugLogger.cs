using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Common
{
    public sealed class DebugLogger : IDebugLogger
    {
        private readonly HashSet<Type> disabledTypes = new();

        public bool IsEnabled { get; set; } = true;

        public void DisableDebugFor(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
                disabledTypes.Add(types[i]);
        }

        public void EnableDebugFor(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
                disabledTypes.Remove(types[i]);
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

        public void PrintException(Exception exception, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

#if UNITY_2017_1_OR_NEWER
            Debug.LogException(exception, context as Object);
#else
            System.Diagnostics.Debug.WriteLine(message);
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

        private static string GetMessage(object message, object? context)
        {
            string messageString = (message?.ToString() ?? string.Empty).TrimEnd('.');

            if (context is not null)
            {
                if (context is Object unityObj)
                    return $"{unityObj.name}: {messageString}.";
                else
                    return $"{context.GetType().Name}: {messageString}.";
            }

            return $"{messageString}.";
        }

        private bool IsPrintAllowed(object? context)
        {
            if (!IsEnabled)
                return false;

            if (context is not null && disabledTypes.Contains(context.GetType()))
                return false;

            return true;
        }
    }
}
