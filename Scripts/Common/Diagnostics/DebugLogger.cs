using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Common.Diagnostics
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

            Debug.Log(GetMessage(message, context), context as Object);
        }

        public void PrintWarning(object message, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

            Debug.LogWarning(GetMessage(message, context), context as Object);
        }

        public void PrintError(object message, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

            Debug.LogError(GetMessage(message, context), context as Object);
        }

        public void PrintException(Exception exception, object? context = null)
        {
            if (!IsPrintAllowed(context))
                return;

            Debug.LogException(exception, context as Object);
        }

        public void AssertLog(bool condition, object message, object? context)
        {
            if (!IsPrintAllowed(context) || !condition)
                return;

            Debug.Log(GetMessage(message, context), context as Object);
        }

        public void AssertWarning(bool condition, object message, object? context)
        {
            if (!IsPrintAllowed(context) || !condition)
                return;

            Debug.LogWarning(GetMessage(message, context), context as Object);
        }

        public void AssertError(bool condition, object message, object? context)
        {
            if (!IsPrintAllowed(context) || !condition)
                return;

            Debug.LogError(GetMessage(message, context), context as Object);
        }

        public void AssertException(bool condition, Exception exception, object? context)
        {
            if (!IsPrintAllowed(context) || !condition)
                return;

            Debug.LogException(exception, context as Object);
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
