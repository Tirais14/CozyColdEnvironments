using CommunityToolkit.Diagnostics;
using System;
using UnityEngine.Animations;

#nullable enable
namespace CCEnvs.Services
{
    public class OnResolvedAction
    {
        private readonly Delegate action;

        public OnResolvedAction(Action<object> action)
            :
            this((Delegate)action)
        {
        }

        protected OnResolvedAction(Delegate action)
        {
            Guard.IsNotNull(action, nameof(action));
            this.action = action;
        }

        public Delegate AsDelegate() => action;

        public void Execute(object instance) => action.DynamicInvoke(instance);
    }
    public class OnResolvedAction<T> : OnResolvedAction
    {
        public OnResolvedAction(Action<T> action) : base(action) { }
    }
}
