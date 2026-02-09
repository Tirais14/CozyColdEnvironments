#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs.Reflection.Caching
{
    public static class TypeCache
    {
    }

    public static class TypeCache<T>
    {
        #region IL2CPP Type definitions

        private readonly static T[]? m_Array;

        private readonly static List<T>? m_List;

        private readonly static HashSet<T>? m_HashSet;

        private readonly static Queue<T>? m_Queue;

        private readonly static Stack<T>? m_Stack;

        #endregion

        public static string String { get; } = CachedTypeof<T>.Type.ToString();

        public static bool IsUnityObject { get; }
        public static bool IsCCBheaviour { get; }

        static TypeCache()
        {
            ResolveTypeInheritanceFlags(
                CachedTypeof<T>.Type,
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
