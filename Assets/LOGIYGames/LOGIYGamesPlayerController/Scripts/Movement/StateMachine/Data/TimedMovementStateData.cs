using System;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Base class for states with timer support (duration + cooldown)
    /// </summary>
    [Serializable]
    public class TimedMovementStateData : MovementStateData
    {
        [Header("Timing")]
        [Tooltip("Minimum duration in this state before can transition out")]
        public float Duration = 0.5f;

        [Tooltip("Cooldown before can re-enter this state")]
        public float Cooldown = 0.2f;
    }
}
