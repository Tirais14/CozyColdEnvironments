using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System.Text;

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

        public static string CombineID(params IState[] states)
        {
            Guard.IsNotNull(states, nameof(states));

            var sb = new StringBuilder();

            for (int i = 0; i < states.Length; i++)
            {
                sb.Append(states[i].ID);

                if (i != 0)
                    sb.Append('_');
            }

            return sb.ToString();
        }
    }

    public partial class State : IState
    {
        public string ID { get; }

        protected bool IsEntered { get; private set; }

        public State()
        {
            ID = GetType().GetName(TypeNameConvertingAttributes.None);
        }

        public State(string? id)
        {
            ID = id ?? GetType().GetName(TypeNameConvertingAttributes.None);
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

        void IState.Enter()
        {
            IsEntered = true;
            Enter();
        }

        void IState.Exit()
        {
            IsEntered = false;
            Exit();
        }
    }
}
