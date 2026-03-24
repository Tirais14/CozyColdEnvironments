using CCEnvs.Collections;
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
                SetState(transition.NextState.ID);

            currentNode?.State.Tick();
        }

        public void FixedTick()
        {
            currentNode?.State.FixedTick();
        }

        public void LateTick()
        {
            currentNode?.State?.LateTick();
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

        public void SetState(string id)
        {
            Guard.IsNotNull(id);

            if (currentNode.IsNotNull())
            {
                currentNode.State.Exit();

                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog($"State exited. State: {currentNode}");
            }

            currentNode = null;

            var nextNode = nodes[id];

            if (nextNode.IsNotNull())
            {
                nextNode.State.Enter();

                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog($"State entered. State: {nextNode}");
            }

            currentNode = nextNode;    
        }

        public IStateNode GetOrCreateNode(
            IState state,
            IEnumerable<IStateTransition>? transitions = null
            )
        {
            CC.Guard.IsNotNull(state, nameof(state));
            CC.Guard.IsNotNull(transitions, nameof(transitions));

            if (!nodes.TryGetValue(state.ID, out var node))
            {
                if (transitions.IsNullOrEmpty())
                    node = new StateNode(state);
                else
                    node = new StateNode(state, transitions);

                nodes[state.ID] = node;
            }

            return node;
        }

        public void AddNode(IStateNode node)
        {
            CC.Guard.IsNotNull(node, nameof(node));

            nodes[node.State.ID] = node;
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

        public void AddTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            anyTransitions.Add(transition);
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
