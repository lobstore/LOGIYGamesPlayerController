using System;
using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Attack state - AI attacks the target when in range
    /// </summary>
    public class AIAttackState : AIBaseState
    {
        private readonly float _attackCooldown;
        private readonly bool _shouldStrafe;
        private float _attackTimer;
        private float _strafeDirection = 1f;
        private float _strafeChangeTimer;

        public Action OnAttackPerformed;

        public AIAttackState(AIBrainStateDriver brain, float attackCooldown = 1f, bool shouldStrafe = true) : base(brain)
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
                Stop();
                return;
            }

            // Face the target
            Vector3 directionToTarget = Brain.GetDirectionToTarget();
            if (directionToTarget.magnitude > 0.1f)
            {
                CharacterTransform.rotation = Quaternion.LookRotation(directionToTarget);
            }

            // Strafe movement
            if (_shouldStrafe && IsTargetInAttackRange(Brain.Target))
            {
                Vector3 right = CharacterTransform.right;
                Vector3 strafePosition = CharacterTransform.position + right * _strafeDirection * 2f;
                MoveToPosition(strafePosition);
            }
            else
            {
                float distance = Brain.GetDistanceToTarget();

                if (distance > AttackRange * 0.8f)
                {
                    MoveToPosition(Brain.Target.position);
                }
                else if (distance < AttackRange * 0.3f)
                {
                    Vector3 backAwayPosition = CharacterTransform.position - directionToTarget * 2f;
                    MoveToPosition(backAwayPosition);
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Performs the attack action
        /// </summary>
        private void PerformAttack()
        {
            OnAttackPerformed?.Invoke();
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
