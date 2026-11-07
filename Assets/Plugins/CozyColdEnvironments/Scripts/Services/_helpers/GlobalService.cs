using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Services
{
    public class GlobalService<T>
        where T : class
    {
        private static readonly WeakReference<T?> valueRef = new(default);

        private static object? resolvedID;

        public static bool IsResolved { get; private set; }

        static GlobalService()
        {
            CCProjectHelper.SubscribeOnInstallIfNot<GlobalService<T>>(Reset);
        }

        public static T GetValue(object? id = null)
        {
            if (!IsResolved ||
                !valueRef.TryGetTarget(out T? value) ||
                value.IsNull() ||
                IsIDChanged(id))
            {
                value = CCServices.Resolve<T>(id);
                valueRef.SetTarget(value);
                IsResolved = value.IsNotNull();
                resolvedID = id;
                CCServices.UnregisterOnUnbindCallback(typeof(T), Reset, id);
                CCServices.RegisterOnUnbindCallback(typeof(T), Reset, id);
            }

            return value!;
        }

        public static bool TryGetValue([NotNullWhen(true)] out T? result, object? id = null)
        {
            if (!IsResolved ||
                !valueRef.TryGetTarget(out T? value) ||
                value.IsNull() ||
                IsIDChanged(id))
            {
                if (CCServices.TryResolveOut(out value, id))
                {
                    valueRef.SetTarget(value);
                    IsResolved = value.IsNotNull();
                    resolvedID = id;
                    CCServices.RegisterOnUnbindCallback(typeof(T), Reset, id); 
                }
            }

            result = value;
            return IsResolved;
        }

        public static void Reset()
        {
            valueRef.SetTarget(null);
            IsResolved = false;
        }

        private static bool IsIDChanged(object? otherID)
        {
            return !EqualityComparer<object?>.Default.Equals(otherID, resolvedID);
        }
    }
}
