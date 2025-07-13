using System;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Patterns.States
{
    public abstract class StateBase : IStateBase
    {
        private readonly Flags<Type> transitionTypes;

        protected StateBase(params Type[] transitionTypes)
        {
            this.transitionTypes = new Flags<Type>(transitionTypes);
        }

        public abstract void Enter();

        public abstract void Exit();

        /// <exception cref="ArgumentNullException"></exception>
        public bool CanTransitionTo(IStateBase stateBase)
        {
            if (stateBase.IsNull())
                throw new ArgumentNullException();

            return transitionTypes.Contains(stateBase.GetType());
        }
    }
}
