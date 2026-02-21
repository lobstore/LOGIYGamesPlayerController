using System;
using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Attack state - AI attacks the target when in range
    /// Transitions back to Chase when target moves out of range
    /// </summary>
    public class AIAttackState : AIBaseState
    {
        /// <summary>
        /// Time between attacks
        /// </summary>
        private float _attackCooldown = 1f;

        /// <summary>
        /// Timer for attack cooldown
        /// </summary>
        private float _attackTimer;

        /// <summary>
        /// Action invoked when AI performs an attack
        /// </summary>
        public Action OnAttackPerformed;

        /// <summary>
        /// Whether AI should strafe during attack
        /// </summary>
        private bool _shouldStrafe = true;

        /// <summary>
        /// Strafe direction (1 = right, -1 = left)
        /// </summary>
        private float _strafeDirection = 1f;

        /// <summary>
        /// Time before changing strafe direction
        /// </summary>
        private float _strafeChangeTimer;

        public AIAttackState(AIBrain brain, float attackCooldown = 1f, bool shouldStrafe = true) : base(brain)
        {
            _attackCooldown = attackCooldown;
            _shouldStrafe = shouldStrafe;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            _attackTimer = _attackCooldown; // Allow immediate first attack
            _strafeChangeTimer = 0f;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if target is null or out of range - transition to Chase
            if (Brain.Target == null || !IsTargetInAttackRange(Brain.Target))
            {
                return;
            }

            // Check line of sight
            if (!HasLineOfSight(Brain.Target, AttackRange))
            {
                return;
            }

            // Handle attack cooldown
            _attackTimer += Time.deltaTime;

            // Perform attack when ready
            if (_attackTimer >= _attackCooldown)
            {
                PerformAttack();
                _attackTimer = 0f;
            }

            // Handle strafing behavior
            if (_shouldStrafe)
            {
                _strafeChangeTimer += Time.deltaTime;
                if (_strafeChangeTimer > 2f)
                {
                    _strafeChangeTimer = 0f;
                    _strafeDirection = -_strafeDirection;
                }
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

            // Face the target
            Vector3 directionToTarget = GetDirectionToTarget(Brain.Target);
            if (directionToTarget.magnitude > 0.1f)
            {
                Character.RotateToDirection(directionToTarget);
            }

            // Strafe movement (optional)
            if (_shouldStrafe && IsTargetInAttackRange(Brain.Target))
            {
                Vector3 right = AITransform.right;
                Vector3 strafeDirection = right * _strafeDirection;
                SetMovementDirection(strafeDirection);
            }
            else
            {
                // Small adjustments to stay in attack range
                float distance = GetDistanceToTarget(Brain.Target);
                
                if (distance > AttackRange * 0.8f)
                {
                    MoveTowardsPosition(Brain.Target.position);
                }
                else if (distance < AttackRange * 0.3f)
                {
                    // Back away slightly
                    MoveTowardsPosition(AITransform.position - directionToTarget * 2f);
                }
                else
                {
                    AIInput.SetMovementInput(Vector2.zero);
                }
            }
        }

        /// <summary>
        /// Performs the attack action
        /// </summary>
        private void PerformAttack()
        {
            AIInput.PressAttack();
            OnAttackPerformed?.Invoke();
            
            // Release attack after a short delay
            // In a real implementation, this would be tied to animation events
            ReleaseAttackAfterDelay(0.2f);
        }

        /// <summary>
        /// Releases attack after a delay
        /// </summary>
        private async void ReleaseAttackAfterDelay(float delay)
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(delay));
            AIInput.ReleaseAttack();
        }

        /// <summary>
        /// Sets the attack cooldown time
        /// </summary>
        public void SetAttackCooldown(float cooldown)
        {
            _attackCooldown = cooldown;
        }

        /// <summary>
        /// Enables or disables strafing behavior
        /// </summary>
        public void SetStrafingEnabled(bool enabled)
        {
            _shouldStrafe = enabled;
        }

        /// <summary>
        /// Gets the current attack cooldown timer
        /// </summary>
        public float GetAttackTimer()
        {
            return _attackTimer;
        }

        /// <summary>
        /// Gets the attack cooldown duration
        /// </summary>
        public float GetAttackCooldown()
        {
            return _attackCooldown;
        }

        /// <summary>
        /// Checks if AI can attack (cooldown is ready)
        /// </summary>
        public bool CanAttack()
        {
            return _attackTimer >= _attackCooldown;
        }
    }
}
