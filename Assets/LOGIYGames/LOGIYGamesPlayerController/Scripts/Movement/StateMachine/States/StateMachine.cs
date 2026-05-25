using System;
using System.Collections.Generic;
namespace LOGIYGames
{
    public class StateMachine
    {
        public StateNode CurrentNode { get; private set; }

        public string LastTransition { get; private set; } = "";

        private readonly Dictionary<Type, StateNode> _nodes = new();

        private readonly HashSet<ITransition> _anyTransitions = new();

        // =====================================================
        // UPDATE
        // =====================================================

        public void Update()
        {
            var transition = GetTransition();

            if (transition != null)
            {
                ChangeState(transition.To);
            }

            CurrentNode?.State?.LogicUpdate();
        }

        public void FixedUpdate()
        {
            CurrentNode?.State?.PhysicsUpdate();
        }

        public void LateUpdate()
        {
            CurrentNode?.State?.LateUpdate();
        }

        // =====================================================
        // STATES
        // =====================================================

        public void AddState(IState state)
        {
            var type = state.GetType();

            if (_nodes.ContainsKey(type))
                return;

            _nodes.Add(type, new StateNode(state));
        }

        public void RemoveState<T>()
            where T : IState
        {
            var type = typeof(T);

            if (!_nodes.ContainsKey(type))
                return;

            // Remove all transitions TO this state
            foreach (var node in _nodes.Values)
            {
                node.Transitions.RemoveWhere(
                    t => t.To == type);
            }

            // Remove any transitions TO this state
            _anyTransitions.RemoveWhere(
                t => t.To == type);

            // Exit current state if needed
            if (CurrentNode != null &&
                CurrentNode.State.GetType() == type)
            {
                CurrentNode.State.Exit();
                CurrentNode = null;
            }

            _nodes.Remove(type);
        }

        public bool HasState<T>()
            where T : IState
        {
            return _nodes.ContainsKey(typeof(T));
        }

        public T GetState<T>()
            where T : class, IState
        {
            if (_nodes.TryGetValue(typeof(T), out var node))
            {
                return node.State as T;
            }

            return null;
        }

        // =====================================================
        // SET / CHANGE STATE
        // =====================================================

        public void SetState<T>()
            where T : IState
        {
            var type = typeof(T);

            if (!_nodes.TryGetValue(type, out var node))
                return;

            CurrentNode = node;

            CurrentNode.State.Enter();
        }

        public void ChangeState<T>()
            where T : IState
        {
            ChangeState(typeof(T));
        }

        private void ChangeState(Type type)
        {
            if (!_nodes.TryGetValue(type, out var nextNode))
                return;

            if (CurrentNode == nextNode)
                return;

            var previousState = CurrentNode?.State;

            previousState?.Exit();

            nextNode.State.Enter();

            LastTransition =
                $"{previousState?.GetType().Name} -> {nextNode.State.GetType().Name}";

            CurrentNode = nextNode;
        }

        // =====================================================
        // TRANSITIONS
        // =====================================================

        public void AddTransition<TFrom, TTo>(
            IPredicate condition)
            where TFrom : IState
            where TTo : IState
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);

            if (!_nodes.TryGetValue(fromType, out var fromNode))
                return;

            if (!_nodes.ContainsKey(toType))
                return;

            fromNode.AddTransition(toType, condition);
        }

        public void AddAnyTransition<TTo>(
            IPredicate condition)
            where TTo : IState
        {
            var toType = typeof(TTo);

            if (!_nodes.ContainsKey(toType))
                return;

            _anyTransitions.Add(
                new Transition(toType, condition));
        }

        public void RemoveTransition<TFrom, TTo>()
            where TFrom : IState
            where TTo : IState
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);

            if (!_nodes.TryGetValue(fromType, out var fromNode))
                return;

            fromNode.Transitions.RemoveWhere(
                t => t.To == toType);
        }

        public void RemoveAnyTransition<TTo>()
            where TTo : IState
        {
            var toType = typeof(TTo);

            _anyTransitions.RemoveWhere(
                t => t.To == toType);
        }

        // =====================================================
        // HELPERS
        // =====================================================

        public bool IsInState<T>()
            where T : IState
        {
            if (CurrentNode == null)
                return false;

            return CurrentNode.State.GetType() == typeof(T);
        }

        private ITransition GetTransition()
        {
            // Any transitions
            foreach (var transition in _anyTransitions)
            {
                if (!HasTransitionTarget(transition))
                    continue;

                if (transition.Condition.Evaluate())
                    return transition;
            }

            // State transitions
            if (CurrentNode == null)
                return null;

            foreach (var transition in CurrentNode.Transitions)
            {
                if (!HasTransitionTarget(transition))
                    continue;

                if (transition.Condition.Evaluate())
                    return transition;
            }

            return null;
        }

        private bool HasTransitionTarget(
            ITransition transition)
        {
            return _nodes.ContainsKey(transition.To);
        }

        // =====================================================
        // NODE
        // =====================================================

        public class StateNode
        {
            public IState State { get; }

            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;

                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(
                Type to,
                IPredicate condition)
            {
                Transitions.Add(
                    new Transition(to, condition));
            }
        }
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

        public void AddTransition(Type to, IPredicate condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }
}