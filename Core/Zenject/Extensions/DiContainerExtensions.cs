using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Linq;
using Zenject;

#nullable enable

namespace UTIRLib.Zenject
{
    public static class DiContainerExtensions
    {
        public static IdScopeConcreteIdArgConditionCopyNonLazyBinder? BindFromScene<T>(this DiContainer container,
            bool optional = false)
            where T : Object
        {
            T? value = Object.FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (value != null)
            {
                return container.BindInstance(value);
            }
            else if (!optional)
            {
                throw new ObjectNotFoundException(typeof(T));
            }

            return null;
        }

        public static IdScopeConcreteIdArgConditionCopyNonLazyBinder? BindFromScene<T, TContract>(
            this DiContainer container, bool optional = false)
            where T : Object
        {
            TContract? value = Object.FindAnyObjectByType<T>(FindObjectsInactive.Include).IsQ<Object, TContract>();
            if (value.IsNotDefault())
            {
                return container.BindInstance(value);
            }
            else if (!optional)
            {
                throw new System.Exception($"Error while binding {typeof(T).Name} as {typeof(TContract).Name}.");
            }

            return null;
        }
    }
}