using System;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Linq;
using Zenject;
using Object = UnityEngine.Object;

#nullable enable

namespace UTIRLib.Zenject
{
    public static class DiContainerExtensions
    {
        [Obsolete("Use FromMethod instead")]
        public static IdScopeConcreteIdArgConditionCopyNonLazyBinder BindFromScene<T>(
            this DiContainer container,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
            where T : Object
        {
            T? value = Object.FindAnyObjectByType<T>(findObjectsInactive);

            return value == null ? throw new ObjectNotFoundException(typeof(T)) 
                            : container.BindInstance(value);
        }

        [Obsolete("Use FromMethod instead")]
        public static IdScopeConcreteIdArgConditionCopyNonLazyBinder BindFromScene<T, TContract>(
            this DiContainer container,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
            where T : Object
        {
            TContract? value = Object.FindAnyObjectByType<T>(findObjectsInactive).IsQ<Object, TContract>();

            return value == null ? throw new ObjectNotFoundException(typeof(TContract))
                            : container.BindInstance(value);
        }
    }
}