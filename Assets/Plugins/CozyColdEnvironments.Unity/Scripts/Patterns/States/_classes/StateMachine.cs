using CCEnvs.Diagnostics;
using CCEnvs.Patterns.States;
using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.States
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<string, IStateNode> nodes = new();
        private readonly List<IStateTransition> anyTransitions = new();

        private IStateNode? currentNode;

        public IReadOnlyDictionary<string, IStateNode> Nodes => nodes;

        public IReadOnlyList<IStateTransition> AnyTransitions => anyTransitions;

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

        public void SetState(string? id)
        {
            if (currentNode.IsNotNull())
            {
                currentNode.State.Exit();

                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog($"State exited. State: {currentNode}");
            }

            currentNode = null;

            if (id is null)
                return;

            var nextNode = nodes[id];

            if (nextNode.IsNotNull())
            {
                nextNode.State.Enter();

                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog($"State entered. State: {nextNode}");
            }

            currentNode = nextNode;    
        }

        public void SetState(IState? state)
        {
            SetState(state?.ID);
        }

        public IStateMachine AddNode(IStateNode node)
        {
            CC.Guard.IsNotNull(node, nameof(node));

            nodes[node.State.ID] = node;
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

        public bool RemoveNode(string id)
        {
            Guard.IsNotNull(id, nameof(id));

            return nodes.Remove(id);
        }

        public bool ContainsNode(string? id)
        {
            if (id is null)
                return false;

            return nodes.ContainsKey(id);
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
    }
}
