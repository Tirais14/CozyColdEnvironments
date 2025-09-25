#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Utils;
using System;
using UnityEngine;
using static CCEnvs.Unity.Extenject.InjectionWrapping;
using Object = UnityEngine.Object;

namespace CCEnvs.Unity.Extenject
{
    public static class InjectionWrapping
    {
        public readonly struct FindAnyObjectByType
        {
        }

        public readonly struct GetComponent
        {
            public readonly Type type;
            public readonly bool inParent;
            public readonly bool inChildren;

            public GetComponent(Type type) : this()
            {
                this.type = type;
            }

            public GetComponent(Type type, InParent _)
                :
                this(type)
            {
                inParent = true;
            }

            public GetComponent(Type type, InChildren _)
                :
                this(type)
            {
                inChildren = true;
            }

            public readonly struct InParent
            {
            }

            public readonly struct InChildren
            {
            }
        }
    }
    public abstract class InjectionWrapping<T> : IInjectionWrapping<T>
    {
        public T Value { get; }

        protected InjectionWrapping(T value)
        {
            Value = value;
        }

        protected InjectionWrapping(Func<T> factory)
        {
            Value = factory();
        }

        /// <exception cref="ObjectNotFoundException"></exception>
        protected InjectionWrapping(FindAnyObjectByType findAnyObjectByType)
        {
            Value = Object.FindAnyObjectByType(typeof(T)).AsOrDefault<T>()!;

            if (Value.IsNull())
                throw new ObjectNotFoundException(typeof(T));
        }

        protected InjectionWrapping(FindAnyObjectByType findAnyObjectByType,
            GetComponent getComponent)
        {
            var fromComponent = SceneObjectSearch.FindObjectByType(getComponent.type, FindObjectsSortMode.None) as Component;

            if (fromComponent.IsNull())
                throw new ObjectNotFoundException(getComponent.type);

            if (getComponent.inParent)
                Value = fromComponent.gameObject.GetComponentInParent<T>();
            else if (getComponent.inChildren)
                Value = fromComponent.gameObject.GetComponentInChildren<T>();
            else
                Value = fromComponent.gameObject.GetComponent<T>();

            if (Value.IsNull())
                throw new ObjectNotFoundException(typeof(T));
        }

        /// <exception cref="ObjectNotFoundException"></exception>
        protected InjectionWrapping(FindAnyObjectByType findAnyObjectByType,
                                    string gameObjectName)
        {
            var go = GameObject.Find(gameObjectName);

            Value = go.GetComponent(typeof(T)).AsOrDefault<T>()!;

            if (Value.IsNull())
                throw new ObjectNotFoundException(typeof(T));
        }

        public static implicit operator T(InjectionWrapping<T> wrapper)
        {
            return wrapper.Value;
        }
    }
}
