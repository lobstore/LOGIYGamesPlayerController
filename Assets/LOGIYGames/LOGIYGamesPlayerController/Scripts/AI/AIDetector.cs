using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// Helper class for AI target detection
    /// Handles range checks and line of sight
    /// </summary>
    public class AIDetector
    {
        private readonly Transform _transform;
        private readonly float _detectionRange;
        private readonly float _attackRange;
        private readonly float _raycastHeight = 0.5f;

        public AIDetector(Transform transform, float detectionRange, float attackRange)
        {
            _transform = transform;
            _detectionRange = detectionRange;
            _attackRange = attackRange;
        }

        /// <summary>
        /// Checks if target is detected (in range and line of sight)
        /// </summary>
        public bool IsTargetDetected(Transform target)
        {
            if (target == null) return false;

            float distance = GetDistanceToTarget(target);
            if (distance > _detectionRange) return false;

            return HasLineOfSightInternal(target, distance);
        }

        /// <summary>
        /// Checks if target is in attack range
        /// </summary>
        public bool IsTargetInAttackRange(Transform target)
        {
            return target != null && GetDistanceToTarget(target) <= _attackRange;
        }

        /// <summary>
        /// Checks if target has been lost (too far)
        /// </summary>
        public bool HasLostTarget(Transform target, float timeoutMultiplier = 1.5f)
        {
            if (target == null) return true;
            return GetDistanceToTarget(target) > _detectionRange * timeoutMultiplier;
        }

        /// <summary>
        /// Gets distance to target
        /// </summary>
        public float GetDistanceToTarget(Transform target)
        {
            if (target == null) return float.MaxValue;
            return Vector3.Distance(_transform.position, target.position);
        }

        /// <summary>
        /// Gets direction to target (flattened)
        /// </summary>
        public Vector3 GetDirectionToTarget(Transform target)
        {
            if (target == null) return _transform.forward;

            Vector3 direction = target.position - _transform.position;
            direction.y = 0;
            return direction.normalized;
        }

        /// <summary>
        /// Checks line of sight to target (internal, distance already known)
        /// </summary>
        private bool HasLineOfSightInternal(Transform target, float distance)
        {
            Vector3 direction = target.position - _transform.position;
            direction.Normalize();

            if (Physics.Raycast(_transform.position + Vector3.up * _raycastHeight, direction, out RaycastHit hit, distance))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return true;
        }

        /// <summary>
        /// Checks if AI can see target with custom max distance
        /// </summary>
        public bool HasLineOfSight(Transform target, float maxDistance)
        {
            if (target == null) return false;

            Vector3 direction = target.position - _transform.position;
            float distance = direction.magnitude;

            if (distance > maxDistance) return false;

            direction.Normalize();

            if (Physics.Raycast(_transform.position + Vector3.up * _raycastHeight, direction, out RaycastHit hit, distance))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return true;
        }
    }
}
