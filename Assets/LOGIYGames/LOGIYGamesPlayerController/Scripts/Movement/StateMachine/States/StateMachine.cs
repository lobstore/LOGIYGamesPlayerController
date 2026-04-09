using System;
using System.Collections.Generic;
namespace LOGIYGames
{
    public class StateMachine
    {
        public StateNode CurrentNode { get; private set; }
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();
        public string LastTransition { get; private set; } = "";
        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);

            CurrentNode.State?.LogicUpdate();
        }

        public void FixedUpdate()
        {
            CurrentNode.State?.PhysicsUpdate();
        }

        public void LateUpdate()
        {
            CurrentNode.State?.LateUpdate();
        }

        public void SetState(IState state)
        {

            CurrentNode = nodes[state.GetType()];
            CurrentNode.State?.Enter();
        }

        public void ChangeState(IState state)
        {
            if (state == CurrentNode.State) return;

            var previousState = CurrentNode.State;
            var nextState = nodes[state.GetType()].State;

            previousState?.Exit();
            nextState?.Enter();
            LastTransition = previousState.GetType() + " -> " + nextState.GetType();
            CurrentNode = nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;

            foreach (var transition in CurrentNode.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;

            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }
        public void RemoveNodeIfExist(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());
            if (node == null) return;
            nodes.Remove(state.GetType());
        }
        public class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}