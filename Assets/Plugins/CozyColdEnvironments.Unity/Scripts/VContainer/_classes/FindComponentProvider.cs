using CCEnvs.UnityX;
using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;
using VContainer;

#nullable enable
namespace CCEnvs.Unity.VContainer
{
    public class FindComponentProvider : IInstanceProvider
    {
        private readonly Type componentType;

        private readonly Transform? parent;

        private readonly bool includeInactive;

        public FindComponentProvider(Type componentType, Transform? parent = null, bool includeInactive = false)
        {
            Guard.IsNotNull(componentType, nameof(componentType));

            this.componentType = componentType;
            this.parent = parent;
        }

        public object SpawnInstance(IObjectResolver resolver)
        {
            if (parent != null)
                return parent.Q().IncludeInactive(includeInactive).Component(componentType).Strict();
            else
                return GameObjectQuery.Scene.IncludeInactive(includeInactive).Component(componentType).Strict();
        }
    }
}
