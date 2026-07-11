using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Services
{
    public class GlobalService<T>
        where T : class
    {
        private static T? value;

#if CC_DEBUG_ENABLED
        private static object? resolvedID;
#endif

        public static bool IsResolved { get; private set; }

        static GlobalService()
        {
            CCProjectHelper.SubscribeOnInstallIfNot<GlobalService<T>>(
                () =>
                {
                    value = default;
                    IsResolved = false;
                });
        }

        public static T GetValue(object? id = null)
        {
            if (!IsResolved)
            {
                value = CCServices.Resolve<T>(id);
                IsResolved = true;

#if CC_DEBUG_ENABLED
                resolvedID = value;
#endif
            }
#if CC_DEBUG_ENABLED
            else ValidateID(id);
#endif

            return value!;
        }

        public static bool TryGetValue([NotNullWhen(true)] out T? result, object? id = null)
        {
            if (!IsResolved)
            {
                if (CCServices.TryResolveOut(out value, id))
                {
                    IsResolved = true;

#if CC_DEBUG_ENABLED
                    resolvedID = value;
#endif
                }
            }
#if CC_DEBUG_ENABLED
            else ValidateID(id);
#endif

            result = value;
            return IsResolved;
        }

#if CC_DEBUG_ENABLED
        private static void ValidateID(object? otherID)
        {
            if (otherID.IsNotNull() && !EqualityComparer<object?>.Default.Equals(resolvedID, otherID))
                typeof(GlobalService<T>).PrintWarning("Static service ignores other id after resolve. It must be null or the same id");
        }
#endif
    }
}
