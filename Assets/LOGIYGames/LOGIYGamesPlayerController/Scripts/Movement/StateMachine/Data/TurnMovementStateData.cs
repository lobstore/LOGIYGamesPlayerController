using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class TurnMovementStateData : TimedMovementStateData
    {
        [Header("TriggerAngle")]
        [Tooltip("Minimal Difference in DesiredDirection and actual GameObject Direction")]
        public float MinAngle;
        [Tooltip("Maximal Difference in DesiredDirection and actual GameObject Direction")]
        public float MaxAngle;
    }
}
