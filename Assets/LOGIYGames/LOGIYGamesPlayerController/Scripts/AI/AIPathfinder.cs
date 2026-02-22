using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// Helper class for AI pathfinding using NavMeshAgent
    /// Encapsulates path calculation and direction extraction
    /// </summary>
    public class AIPathfinder
    {
        private readonly UnityEngine.AI.NavMeshAgent _agent;
        private readonly Transform _transform;
        private float _pathRecalculateTimer;
        private readonly float _recalculateInterval;
        private bool _wasGrounded = true;

        public AIPathfinder(UnityEngine.AI.NavMeshAgent agent, Transform transform, float recalculateInterval = 0.3f)
        {
            _agent = agent;
            _transform = transform;
            _recalculateInterval = recalculateInterval;
        }

        /// <summary>
        /// Updates path to destination with throttling
        /// </summary>
        public void UpdatePath(Vector3 destination, bool isGrounded = true)
        {
            if (!isGrounded)
            {
                _wasGrounded = false;
                return;
            }

            if (!_wasGrounded)
            {
                _wasGrounded = true;
                _pathRecalculateTimer = _recalculateInterval;
            }

            _pathRecalculateTimer += Time.deltaTime;
            if (_pathRecalculateTimer >= _recalculateInterval)
            {
                _pathRecalculateTimer = 0f;
                if (_agent != null && _agent.isOnNavMesh)
                {
                    _agent.SetDestination(destination);
                }
            }
        }

        /// <summary>
        /// Gets movement direction from current NavMesh path
        /// </summary>
        public Vector3 GetDirection()
        {
            if (_agent == null || !_agent.isOnNavMesh || !_agent.hasPath || _agent.pathPending)
            {
                return Vector3.zero;
            }

            if (_agent.path.corners != null && _agent.path.corners.Length > 1)
            {
                Vector3 nextWaypoint = _agent.path.corners[1];
                Vector3 direction = nextWaypoint - _transform.position;
                direction.y = 0;
                return direction.normalized;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Forces immediate path recalculation
        /// </summary>
        public void Recalculate()
        {
            _pathRecalculateTimer = _recalculateInterval;
        }

        /// <summary>
        /// Checks if path is available and valid
        /// </summary>
        public bool HasValidPath => _agent != null && _agent.isOnNavMesh && _agent.hasPath && !_agent.pathPending;
    }
}
