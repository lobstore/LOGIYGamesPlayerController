using UnityEngine;

namespace LOGIYGames.AI
{
    public sealed class AIChaseState : AIBaseState
    {
        private const float RepathInterval = 0.2f;
        private const float TargetMoveThreshold = 0.5f;

        private float _nextRepathTime;
        private Vector3 _lastTargetPosition;

        public AIChaseState(AIBrain brain) : base(brain)
        {
        }

        public override void Enter()
        {
            _nextRepathTime = 0f;

            UpdateDestination();
        }

        public override void Exit()
        {
        }

        public override void LogicUpdate()
        {
            if (!Brain.HasTarget)
                return;

            bool targetMoved =
                (Brain.Target.position - _lastTargetPosition)
                .sqrMagnitude >=
                TargetMoveThreshold * TargetMoveThreshold;

            if (Time.time >= _nextRepathTime || targetMoved)
            {
                UpdateDestination();
            }
        }

        private void UpdateDestination()
        {
            Transform target = Brain.Target;

            if (target == null)
                return;

            Brain.SetMoveDestination(target.position);

            _lastTargetPosition = target.position;

            _nextRepathTime = Time.time + RepathInterval;
        }
    }
}