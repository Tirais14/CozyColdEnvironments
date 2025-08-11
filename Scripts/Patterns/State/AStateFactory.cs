using System;
using UnityEngine;
using UTIRLib.Patterns.Factory;
using UTIRLib.Patterns.States;

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
}
