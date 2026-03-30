using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Chase state - AI pursues the target
    /// </summary>
    public class AIChaseState : AIBaseState
    {
        private readonly float _lostChaseDuration;
        private float _lostTimer;
        private bool _wasTargetVisible;

        public AIChaseState(AIBrain brain, float lostChaseDuration = 3f) : base(brain)
        {
            _lostChaseDuration = lostChaseDuration;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            _lostTimer = 0f;
            _wasTargetVisible = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Brain.Target == null)
            {
                _lostTimer += Time.deltaTime;
                return;
            }

            bool canSeeTarget = Brain.HasLineOfSight();

            if (canSeeTarget)
            {
                _lostTimer = 0f;
                _wasTargetVisible = true;
            }
            else if (_wasTargetVisible)
            {
                _lostTimer += Time.deltaTime;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (Brain.Target == null)
            {
                Stop();
                return;
            }

            float distanceToTarget = Brain.GetDistanceToTarget();

            // If too close, stop
            if (distanceToTarget <= AttackRange * 0.5f)
            {
                Stop();
                return;
            }

            // Move towards target
            MoveToPosition(Brain.Target.position);
        }
    }
}
