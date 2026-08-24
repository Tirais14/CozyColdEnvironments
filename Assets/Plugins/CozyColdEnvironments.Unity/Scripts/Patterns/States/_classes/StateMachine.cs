using CCEnvs.Diagnostics;
using CCEnvs.Patterns.States;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.States
{
    public class StateMachine : IStateMachine, IDisposable
    {
        private readonly Dictionary<Type, IStateNode> nodes = new();
        private readonly List<IStateTransition> anyTransitions = new();

        private readonly ReactiveProperty<IStateNode?> _currentNode = new();

        public IReadOnlyDictionary<Type, IStateNode> Nodes => nodes;

        public IReadOnlyList<IStateTransition> AnyTransitions => anyTransitions;

        public Type? CurrentStateType => currentNode?.State.StateType;

        protected IStateNode? currentNode {
            get => _currentNode.Value;
            private set => _currentNode.Value = value;
        }

        ~StateMachine() => Dispose();

        public void Tick()
        {
            if (ResolveTransition().IsNotNull(out var transition))
                SetState(transition.NextState);

            currentNode?.State.Tick();
        }

        public void FixedTick()
        {
            currentNode?.State.FixedTick();
        }

        public void LateTick()
        {
            currentNode?.State.LateTick();
        }

        public IStateTransition? ResolveTransition()
        {
            for (int i = 0; i < anyTransitions.Count; i++)
                if (anyTransitions[i].Condition.Evaluate())
                    return anyTransitions[i];

            for (int i = 0; i < currentNode?.Transitions.Count; i++)
                if (currentNode.Transitions[i].Condition.Evaluate())
                    return currentNode.Transitions[i];

            return null;
        }

        public void SetState(Type? stateType)
        {
            if (currentNode.IsNotNull())
            {
                currentNode.State.Exit();

                if (CCDebug<StateMachine>.IsEnabled)
                    this.PrintLog($"State exited. State: {currentNode}");
            }

            currentNode = null;

            if (stateType is null)
                return;

            var nextNode = nodes[stateType];

            if (nextNode.IsNotNull())
            {
                nextNode.State.Enter(); 

                if (CCDebug<StateMachine>.IsEnabled)
                    this.PrintLog($"State entered. State: {nextNode}");
            }

            currentNode = nextNode;
        }
        public void SetState<T>() => SetState(typeof(T));

        public void SetState(IState? state)
        {
            SetState(state?.StateType);
        }

        public IStateMachine AddNode(IStateNode node)
        {
            CC.Guard.IsNotNull(node, nameof(node));

            nodes[node.State.StateType] = node;
            return this;
        }

        public IStateMachine AddState(IState state, IStateTransition transition)
        {
            CC.Guard.IsNotNull(state, nameof(state));
            CC.Guard.IsNotNull(transition, nameof(transition));

            var node = new StateNode(state, transition);
            AddNode(node);

            return this;
        }

        public IStateMachine AddState(IState state, params IStateTransition[] transitions)
        {
            CC.Guard.IsNotNull(state, nameof(state));
            CC.Guard.IsNotNull(transitions, nameof(transitions));

            var node = new StateNode(state, transitions);
            AddNode(node);

            return this;
        }

        public bool RemoveNode(Type stateType)
        {
            Guard.IsNotNull(stateType, nameof(stateType));

            return nodes.Remove(stateType);
        }

        public bool ContainsNode(Type? stateType)
        {
            if (stateType is null)
                return false;

            return nodes.ContainsKey(stateType);
        }

        public IStateMachine AddTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            anyTransitions.Add(transition);
            return this;
        }

        public bool RemoveTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            return anyTransitions.Remove(transition);
        }

        public bool ContainsTransition(IStateTransition? transition)
        {
            if (transition.IsNull())
                return false;

            return anyTransitions.Contains(transition);
        }

        public Observable<Type?> ObserveCurrentStateType()
        {
            return _currentNode.Select(node => node?.State.GetType());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private int disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
                _currentNode.Dispose();
        }
    }
}
