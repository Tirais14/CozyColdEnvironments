using CCEnvs.Patterns.Factory;
using System;

#nullable enable
namespace CCEnvs.Patterns.States
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

        where T : IStateMachine
    {
        protected readonly T machine;

        protected AStateFactory(T machine)
        {
            this.machine = machine;
        }
    }
}
