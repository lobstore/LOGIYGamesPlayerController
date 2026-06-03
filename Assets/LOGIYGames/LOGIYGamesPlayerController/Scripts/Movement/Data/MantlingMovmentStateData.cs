using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class MantlingMovmentStateData : TimedMovementStateData
    {
        public float CheckDistance;
        public LayerMask IncludeLayers;
    }
}
