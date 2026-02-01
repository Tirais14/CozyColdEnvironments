using System;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class CachedTypeof<T>
    {
        public static Type Type { get; } = typeof(T);

        public static bool IsUnityObject { get; }
        public static bool IsCCBheaviour { get; }

        static CachedTypeof()
        {
            ResolveTypeInheritanceFlags(
                Type,
                out var isUnityObject,
                out var isCCBehaviour
                );

            IsUnityObject = isUnityObject;
            IsCCBheaviour = isCCBehaviour;
        }

        private static void ResolveTypeInheritanceFlags(Type type, out bool isUnityObject, out bool isCCBehaviour)
        {
            string unityObjectNamepsacePart = "UnityEngine.Object";
            string ccBehaviourNamespacePart = $"{nameof(CCEnvs)}.Unity.CCBehaviour";

            var loopFuse = LoopFuse.Create(iterationLimit: 10000);

            while (type is not null
                   &&
                   loopFuse.DebugMoveNext()
                   )
            {
                if (type.Namespace.IsNullOrWhiteSpace())
                {
                    type = type.BaseType;
                    continue;
                }

                if (type.Namespace.StartsWith(ccBehaviourNamespacePart))
                {
                    isCCBehaviour = true;
                    isUnityObject = true;

                    break;
                }

                if (type.Namespace.StartsWith(unityObjectNamepsacePart))
                {
                    isCCBehaviour = false;
                    isUnityObject = true;

                    break;
                }

                type = type.BaseType;
            }

            isUnityObject = false;
            isCCBehaviour = false;
        }
    }
}
