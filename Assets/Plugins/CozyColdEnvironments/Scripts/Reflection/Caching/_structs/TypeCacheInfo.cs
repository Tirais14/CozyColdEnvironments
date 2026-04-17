using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public struct TypeCacheInfo
    {
        private string? _name;
        private string? _fullName;
        private string? _namespace;

        private bool? isUnityObject;
        private bool? isUnityComponent;
        private bool? isUnityGameObject;
        private bool? isCCBehvaiour;
        private bool? isValueType;
        private bool? isPrimitive;

        public Type Type { get; }

        public bool IsUnityObject {
            get
            {
                if (!isUnityObject.HasValue)
                    InitUnityPart();

                return isUnityObject!.Value;
            }
        }
        public bool IsUnityComponent {
            get
            {
                if (!isUnityComponent.HasValue)
                    InitUnityPart();

                return isUnityComponent!.Value;
            }
        }
        public bool IsUnityGameObject {
            get
            {
                if (!isUnityGameObject.HasValue)
                    InitUnityPart();

                return isUnityGameObject!.Value;
            }
        }
        public bool IsCCBheaviour {
            get
            {
                if (!isCCBehvaiour.HasValue)
                    InitUnityPart();

                return isCCBehvaiour!.Value; 
            }
        }
        public bool IsValueType {
            get
            {
                isValueType ??= Type.IsValueType;
                return isValueType.Value;
            }
        }
        public bool IsPrimitive {
            get
            {
                isPrimitive ??= Type.IsPrimitive;
                return isPrimitive.Value;
            }
        }

        public string Name {
            get
            {
                _name ??= Type.Name;

                return _name;
            }
        }
        public string FullName {
            get
            {
                _fullName ??= Type.FullName;

                return _fullName;
            }
        }
        public string Namespace {
            get
            {
                _namespace ??= Type.Namespace;

                return _namespace;
            }
        }

        public TypeCacheInfo(Type type)
            :
            this()
        {
            Type = type;
        }

        private void InitUnityPart()
        {
#if UNITY_2017_1_OR_NEWER
            var type = Type;

            if (TypeofCache<UnityEngine.Object>.Type.IsAssignableFrom(type))
            {
                isUnityObject = true;
                isUnityComponent = IsUnityComponentType(type);
                isUnityGameObject = IsUnityGameObjectType(type);

                if (IsUnityComponent)
                {
                    while (type is not null)
                    {
                        if (type.Namespace is null
                            ||
                            type.Name is null)
                        {
                            continue;
                        }

                        if (!IsCCBheaviour)
                        {
                            isCCBehvaiour = IsCCBehaviourType(type);
                            break;
                        }

                        type = type.BaseType;
                    }
                }
            }
#endif
        }

#if UNITY_2017_1_OR_NEWER

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsUnityComponentType(Type type)
        {
            return !IsUnityGameObject && TypeofCache<UnityEngine.Component>.Type.IsAssignableFrom(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsUnityGameObjectType(Type type)
        {
            return !IsUnityComponent && type == TypeofCache<UnityEngine.GameObject>.Type;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly bool IsCCBehaviourType(Type type)
        {
            return type.Namespace.ContainsOrdinal(NamepsaceHelper.NAMESPACE_CCENVS_UNITY_COMPONENTS)
                   &&
                   type.Name.Contains("CCBehaviour");
        }
    }
}
