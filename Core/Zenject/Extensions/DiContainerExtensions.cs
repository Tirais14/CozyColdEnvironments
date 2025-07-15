using NUnit.Framework;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Linq;
using Zenject;

#nullable enable

namespace UTIRLib.Zenject
{
    public static class DiContainerExtensions
    {
        public static IdScopeConcreteIdArgConditionCopyNonLazyBinder BindFromScene<T>(
            this DiContainer container,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
            where T : Object
        {
            T? value = Object.FindAnyObjectByType<T>(findObjectsInactive);

            return value == null ? throw new ObjectNotFoundException(typeof(T)) 
                            : container.BindInstance(value);
        }

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