using CCEnvs.Diagnostics;
using CCEnvs.Patterns.States;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.States
{
    public class StateMachine : IStateMachine, IDisposable
    {
        private readonly Dictionary<string, IStateNode> nodes = new();
        private readonly Dictionary<Type, IStateNode> typedNodes = new();
        private readonly List<IStateTransition> anyTransitions = new();

        private readonly ReactiveProperty<IStateNode?> _currentNode = new();

        public IReadOnlyDictionary<string, IStateNode> Nodes => nodes;

        public IReadOnlyList<IStateTransition> AnyTransitions => anyTransitions;

        public string? CurrentStateID => currentNode?.State.ID;

        protected IStateNode? currentNode {
            get => _currentNode.Value;
            private set => _currentNode.Value = value;
        }

        ~StateMachine() => Dispose();

        public void Tick()
        {
            if (ResolveTransition() is IStateTransition transition)
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

        public IStateNode GetNode(Type stateType)
        {
            Guard.IsNotNull(stateType, nameof(stateType));

            if (TryFindNode(stateType, out var node))
                return node;

            return GetNode(stateType.GetName(TypeNameConvertingAttributes.None));
        }
        public IStateNode GetNode<T>() => GetNode(typeof(T));
        public IStateNode GetNode(string id)
        {
            Guard.IsNotNull(id, nameof(id));
            return nodes[id];
        }

        public void SetState(string? id)
        {
            if (currentNode.IsNotNull())
            {
                currentNode.State.Exit();

                if (CCDebug<StateMachine>.IsEnabled)
                {
                    this.PrintLog(DebugMessageBuilder.CreatePooled()
                        .AddMessage("State exited")
                        .AddProperty("ID", currentNode.State.ID)
                        .ToStringAndDispose()
                        );
                }
            }

            currentNode = null;

            if (id is null)
                return;

            var nextNode = nodes[id];

            if (nextNode.IsNotNull())
            {
                nextNode.State.Enter();

                if (CCDebug<StateMachine>.IsEnabled)
                {
                    this.PrintLog(DebugMessageBuilder.CreatePooled()
                        .AddMessage("State entered")
                        .AddProperty("ID", nextNode.State.ID)
                        .ToStringAndDispose()
                        );
                }
            }

            currentNode = nextNode;
        }
        public void SetState(Type? stateType)
        {
            if (TryFindNode(stateType, out var node))
            {
                SetState(node.State);
                return;
            }    

            SetState(stateType.GetName(TypeNameConvertingAttributes.None));
        }
        public void SetState<T>() => SetState(typeof(T));

        public void SetState(IState? state)
        {
            SetState(state?.ID);
        }

        public IStateMachine AddNode(IStateNode node)
        {
            CC.Guard.IsNotNull(node, nameof(node));

            nodes.Add(node.State.ID, node);
            typedNodes[node.State.GetType()] = node;
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

        public bool RemoveNode(string? id)
        {
            Guard.IsNotNull(id, nameof(id));

            if (id is null
                || !nodes.Remove(id, out var node)
                )
                return false;

            typedNodes.Remove(node.State.GetType());
            return true;
        }
        public bool RemoveNode(Type? stateType)
        {
            if (stateType is null)
                return false;

            if (TryFindNode(stateType, out var node))
                return RemoveNode(node.State.ID);

            return RemoveNode(stateType.GetName(TypeNameConvertingAttributes.None));
        }
        public bool RemoveNode<T>()
        {
            return RemoveNode(typeof(T).GetName(TypeNameConvertingAttributes.None));
        }

        public bool ContainsNode(string? id)
        {
            if (id is null)
                return false;

            return nodes.ContainsKey(id);
        }
        public bool ContainsNode(Type? stateType)
        {
            if (stateType is null)
                return false;

            return TryFindNode(stateType, out _) || ContainsNode(stateType.GetName(TypeNameConvertingAttributes.None));
        }
        public bool ContainsNode<T>() => ContainsNode(typeof(T));

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

        public Observable<string?> ObserveCurrentStateID()
        {
            return _currentNode.Select(node => node?.State.ID);
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

        private bool TryFindNode(Type? stateType, [NotNullWhen(true)] out IStateNode? result)
        {
            if (stateType is null
                || (!typedNodes.TryGetValue(stateType, out var node) 
                && !nodes.TryGetValue(stateType.GetName(TypeNameConvertingAttributes.None), out node)))
            {
                result = null;
                return false;
            }

            result = node;
            return true;
        }
    }
}
