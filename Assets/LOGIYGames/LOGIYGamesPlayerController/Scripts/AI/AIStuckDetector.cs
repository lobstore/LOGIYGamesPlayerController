using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// Helper class for detecting if AI is stuck
    /// </summary>
    public class AIStuckDetector
    {
        private Vector3 _lastPosition;
        private float _stuckTimer;
        private readonly float _stuckThreshold;
        private readonly float _stuckTimeout;

        public AIStuckDetector(float stuckThreshold = 0.1f, float stuckTimeout = 2f)
        {
            _stuckThreshold = stuckThreshold;
            _stuckTimeout = stuckTimeout;
        }

        /// <summary>
        /// Updates stuck detection and returns true if AI is stuck
        /// </summary>
        public bool IsStuck(Vector3 currentPosition)
        {
            float moveDistance = Vector3.Distance(currentPosition, _lastPosition);

            if (moveDistance < _stuckThreshold)
            {
                _stuckTimer += Time.deltaTime;
                return _stuckTimer >= _stuckTimeout;
            }
            else
            {
                _stuckTimer = 0f;
                _lastPosition = currentPosition;
                return false;
            }
        }

        /// <summary>
        /// Resets the stuck detector
        /// </summary>
        public void Reset()
        {
            _stuckTimer = 0f;
        }

        /// <summary>
        /// Gets current stuck timer value
        /// </summary>
        public float StuckTimer => _stuckTimer;
    }
}
