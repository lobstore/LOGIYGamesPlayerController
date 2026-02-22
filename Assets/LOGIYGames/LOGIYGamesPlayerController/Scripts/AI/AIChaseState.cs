using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Chase state - AI pursues the target
    /// Transitions to Attack when in range, or to Patrol/Idle when target is lost
    /// </summary>
    public class AIChaseState : AIBaseState
    {
        private float _lostChaseDuration;
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

        public override void Exit()
        {
            base.Exit();
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
                AIInput.SetMovementInput(Vector2.zero);
                return;
            }

            float distanceToTarget = Brain.GetDistanceToTarget();

            // If too close, stop or back away
            if (distanceToTarget <= AttackRange * 0.5f)
            {
                AIInput.SetMovementInput(Vector2.zero);
                return;
            }

            // Move towards target using NavMesh pathfinding
            MoveAlongNavMeshPath(Brain.Target.position, AttackRange * 0.5f);
        }

        /// <summary>
        /// Gets how long target has been lost
        /// </summary>
        public float GetLostTimer()
        {
            return _lostTimer;
        }

        /// <summary>
        /// Sets the duration AI will chase after losing target
        /// </summary>
        public void SetLostChaseDuration(float duration)
        {
            _lostChaseDuration = duration;
        }
    }
}
