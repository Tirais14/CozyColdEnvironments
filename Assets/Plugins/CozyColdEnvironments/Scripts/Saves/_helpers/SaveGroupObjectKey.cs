using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using CCEnvs.Collections;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveGroupObjectKey
    {
#if UNITY_2017_1_OR_NEWER
        private readonly static Lazy<Type> gameObjectExtraInfoType = new(
            static () =>
            {
                return Type.GetType(
                    "CCEnvs.Unity.GameObjectExtraInfo, CCEnvs.Unity",
                    throwOnError: true
                    );
            });

        private readonly static Lazy<Type> gameObjectExtraInfoExtensionsType = new(
            static () =>
            {
                return Type.GetType(
                    "CCEnvs.Unity.GameObjectExtraInfoExtensions, CCEnvs.Unity",
                    throwOnError: true
                    );
            });

        private readonly static Lazy<MethodInfo> gameObjectExtraInfoExtensions_GetExtraInfo_GO = new(
            static () =>
            {
                var extType = gameObjectExtraInfoExtensionsType.Value;

                if (extType is null)
                    throw new ArgumentException(nameof(extType));

                return extType.GetMethod(
                    "GetExtraInfo",
                    BindingFlagsDefault.StaticPublic,
                    binder: null,
                    new Type[] { typeof(UnityEngine.GameObject) },
                    new arr<ParameterModifier>()
                    )
                    ??
                    throw new InvalidOperationException("Cannot find method");
            });

        private readonly static Lazy<MethodInfo> gameObjectExtraInfoExtensions_GetExtraInfo_Cmp = new(
            static () =>
            {
                var extType = gameObjectExtraInfoExtensionsType.Value;

                if (extType is null)
                    throw new ArgumentException(nameof(extType));

                return extType.GetMethod(
                    "GetExtraInfo",
                    BindingFlagsDefault.StaticPublic,
                    binder: null,
                    new Type[] { typeof(UnityEngine.Component) },
                    new arr<ParameterModifier>()
                    )
                    ??
                    throw new InvalidOperationException("Cannot find method");
            });

        private readonly static Lazy<PropertyInfo> gameObjectExtraInfo_PersistentGUID_Property = new(
            static () =>
            {
                var infoType = gameObjectExtraInfoType.Value;

                if (infoType is null)
                    throw new ArgumentException(nameof(infoType));

                var prop = infoType.GetProperty("PersistenGuid", BindingFlagsDefault.InstancePublic);

                return prop ?? throw new InvalidOperationException("Cannot find property: PersistenGuid");
            });

        private readonly static Lazy<PropertyInfo> gameObjectExtraInfo_HierarchyPath_Property = new(
            static () =>
            {
                var infoType = gameObjectExtraInfoType.Value;

                if (infoType is null)
                    throw new ArgumentException(nameof(infoType));

                var prop = infoType.GetProperty("HierarchyPath", BindingFlagsDefault.InstancePublic);

                return prop ?? throw new InvalidOperationException("Cannot find property: HierarchyPath");
            });

#endif //UNITY_2017_1_OR_NEWER

        public static bool TryResolve(
            object obj,
            [NotNullWhen(true)] out string? key
            )
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            key = obj switch
            {
#if UNITY_2017_1_OR_NEWER
                UnityEngine.GameObject go => ResolveGameObjectKey(go),
                UnityEngine.Component cmp => ResolveComponentKey(cmp),
#endif
                _ => null
            };

            return key is not null;
        }

#if UNITY_2017_1_OR_NEWER
        //TODO: Refactor inter-build communication or not,
        //i dont know what to do with this reflection :<
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ResolveGameObjectKey(UnityEngine.GameObject go)
        {
            var extraInfo = gameObjectExtraInfoExtensions_GetExtraInfo_GO.Value.Invoke(null, new object[] { go });

            var guid = (string?)gameObjectExtraInfo_PersistentGUID_Property.Value.GetValue(extraInfo);

            if (guid.IsNotNullOrWhiteSpace())
                return guid;

            var path = gameObjectExtraInfo_HierarchyPath_Property.Value.GetValue(extraInfo);

            return path.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ResolveComponentKey(UnityEngine.Component cmp)
        {
            var extraInfo = gameObjectExtraInfoExtensions_GetExtraInfo_Cmp.Value.Invoke(null, new object[] { cmp });

            var guid = (string?)gameObjectExtraInfo_PersistentGUID_Property.Value.GetValue(extraInfo);

            if (guid.IsNotNullOrWhiteSpace())
                return guid;

            var path = gameObjectExtraInfo_HierarchyPath_Property.Value.GetValue(extraInfo);

            return path.ToString();
        }
#endif
    }
}
