using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Idle state - AI stays in place and observes surroundings
    /// Transitions to Patrol or Chase when conditions are met
    /// </summary>
    public class AIIdleState : AIBaseState
    {
        /// <summary>
        /// How long AI should stay idle before switching to patrol
        /// </summary>
        private float _idleDuration = 3f;

        /// <summary>
        /// Random idle look around timer
        /// </summary>
        private float _lookAroundTimer;

        public AIIdleState(AIBrain brain, float idleDuration = 3f) : base(brain)
        {
            _idleDuration = idleDuration;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            AIInput.ClearAllInputs();
            _lookAroundTimer = 0f;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if target is detected - transition to Chase
            if (Brain.Target != null && CanDetectTarget())
            {
                return;
            }

            // Random look around behavior
            _lookAroundTimer += Time.deltaTime;
            if (_lookAroundTimer > 2f)
            {
                _lookAroundTimer = 0f;
                // Could add head rotation logic here
            }

            // If idle for too long, may transition to Patrol
            if (StateTime > _idleDuration && Brain.PatrolPoints != null && Brain.PatrolPoints.Length > 0)
            {
                // Signal that idle is complete (transition handled by AIBrain predicates)
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // No movement in idle state
            AIInput.SetMovementInput(Vector2.zero);
        }

        /// <summary>
        /// Checks if target can be detected (in range and line of sight)
        /// </summary>
        private bool CanDetectTarget()
        {
            if (Brain.Target == null) return false;

            float distance = GetDistanceToTarget(Brain.Target);
            
            if (distance <= DetectionRange)
            {
                return HasLineOfSight(Brain.Target, DetectionRange);
            }

            return false;
        }

        /// <summary>
        /// Gets the current idle duration
        /// </summary>
        public float GetIdleDuration()
        {
            return _idleDuration;
        }

        /// <summary>
        /// Sets a new idle duration
        /// </summary>
        public void SetIdleDuration(float duration)
        {
            _idleDuration = duration;
        }
    }
}
