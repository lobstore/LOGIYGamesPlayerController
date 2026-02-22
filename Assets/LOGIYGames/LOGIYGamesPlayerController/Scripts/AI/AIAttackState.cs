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
        private float _attackCooldown;
        private bool _shouldStrafe;
        private float _attackTimer;
        private float _strafeDirection = 1f;
        private float _strafeChangeTimer;

        public Action OnAttackPerformed;

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
            _attackTimer = _attackCooldown;
            _strafeChangeTimer = 0f;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if target is null or out of range
            if (Brain.Target == null || !IsTargetInAttackRange(Brain.Target))
            {
                return;
            }

            // Check line of sight
            if (!Brain.HasLineOfSight())
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

            // Handle strafing
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
            Vector3 directionToTarget = Brain.GetDirectionToTarget();
            if (directionToTarget.magnitude > 0.1f)
            {
                Character.RotateToDirection(directionToTarget);
            }

            // Strafe movement
            if (_shouldStrafe && IsTargetInAttackRange(Brain.Target))
            {
                Vector3 right = AITransform.right;
                SetMovementDirection(right * _strafeDirection);
            }
            else
            {
                float distance = Brain.GetDistanceToTarget();

                if (distance > AttackRange * 0.8f)
                {
                    MoveTowardsPosition(Brain.Target.position);
                }
                else if (distance < AttackRange * 0.3f)
                {
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
        /// Enables or disables strafing
        /// </summary>
        public void SetStrafingEnabled(bool enabled)
        {
            _shouldStrafe = enabled;
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
