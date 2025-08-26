using System;
using UTIRLib.Patterns.Factory;

#nullable enable
namespace UTIRLib.Patterns.States
{
    public abstract class AStateFactory : IFactory<Type, IState>
    {
        public abstract IState Create(Type stateType);

        public T Create<T>()
            where T : IState
        {
            return (T)Create(typeof(T));
        }
    }
    public abstract class AStateFactory<T> 
        :
        AStateFactory

        where T : AStateMachine
    {
        protected readonly T machine;

        protected AStateFactory(T machine)
        {
            this.machine = machine;
        }
    }
}
