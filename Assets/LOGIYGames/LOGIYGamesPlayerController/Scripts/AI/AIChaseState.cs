using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Chase state - AI pursues the target
    /// Transitions to Attack when in range, or to Patrol/Idle when target is lost
    /// </summary>
    public class AIChaseState : AIBaseState
    {
        /// <summary>
        /// How long AI will chase after losing sight of target
        /// </summary>
        private float _lostChaseDuration = 3f;

        /// <summary>
        /// Timer for how long target has been lost
        /// </summary>
        private float _lostTimer;

        /// <summary>
        /// Whether target was visible last frame
        /// </summary>
        private bool _wasTargetVisible;

        /// <summary>
        /// Chase movement speed multiplier
        /// </summary>
        private float _chaseSpeed = 1f;

        /// <summary>
        /// Minimum distance to maintain from target (for ranged AI)
        /// </summary>
        private float _minChaseDistance = 3f;

        public AIChaseState(AIBrain brain, float lostChaseDuration = 3f, float chaseSpeed = 1f) : base(brain)
        {
            _lostChaseDuration = lostChaseDuration;
            _chaseSpeed = chaseSpeed;
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
                if (_lostTimer >= _lostChaseDuration)
                {
                    // Lost target for too long, return to patrol or idle
                    return;
                }
                return;
            }

            bool canSeeTarget = CanDetectTarget();

            if (canSeeTarget)
            {
                _lostTimer = 0f;
                _wasTargetVisible = true;
            }
            else
            {
                if (_wasTargetVisible)
                {
                    // Just lost sight, start timer
                    _lostTimer += Time.deltaTime;
                    if (_lostTimer >= _lostChaseDuration)
                    {
                        // Give up chase
                        return;
                    }
                }
            }

            // Check if in attack range - transition to Attack
            if (IsTargetInAttackRange(Brain.Target))
            {
                return;
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

            float distanceToTarget = GetDistanceToTarget(Brain.Target);

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
        /// Sets the duration AI will chase after losing target
        /// </summary>
        public void SetLostChaseDuration(float duration)
        {
            _lostChaseDuration = duration;
        }

        /// <summary>
        /// Sets the chase speed multiplier
        /// </summary>
        public void SetChaseSpeed(float speed)
        {
            _chaseSpeed = Mathf.Clamp01(speed);
        }

        /// <summary>
        /// Sets minimum chase distance (for ranged AI that keep distance)
        /// </summary>
        public void SetMinChaseDistance(float distance)
        {
            _minChaseDistance = distance;
        }

        /// <summary>
        /// Gets how long target has been lost
        /// </summary>
        public float GetLostTimer()
        {
            return _lostTimer;
        }

        /// <summary>
        /// Checks if AI has lost the target completely
        /// </summary>
        public bool HasLostTarget()
        {
            return _lostTimer >= _lostChaseDuration;
        }
    }
}
