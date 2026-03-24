using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public partial class State
    {
        public static IState Combine(string id, IState state, IState state1)
        {
            Guard.IsNotNull(id, nameof(id));
            CC.Guard.IsNotNull(state, nameof(state));
            CC.Guard.IsNotNull(state1, nameof(state1));

            var composite = new CompositeState(id);

            composite.Add(state)
                .Add(state1);

            return composite;
        }

        public static IState Combine(string id, IState state, params IState[] states)
        {
            Guard.IsNotNull(id, nameof(id));
            CC.Guard.IsNotNull(state, nameof(state));
            Guard.IsNotNull(states, nameof(states));

            var composite = new CompositeState(id);

            composite.Add(state)
                .AddRange(states);

            return composite;
        }

        public static string GetIDFromType(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return type.FullName;
        }

        public static string GetIDFromType<T>()
        {
            return TypeofCache<T>.Type.FullName;
        }
    }

    public partial class State : IState
    {
        public string ID { get; }

        public State()
        {
            ID = GetIDFromType(GetType());
        }

        public virtual void Enter()
        {

        }

        public virtual void Tick()
        {

        }

        public virtual void FixedTick()
        {

        }

        public virtual void LateTick()
        {

        }

        public virtual void Exit()
        {

        }

        public override string ToString()
        {
            return $"{nameof(ID)}: {ID}";
        }
    }
}
