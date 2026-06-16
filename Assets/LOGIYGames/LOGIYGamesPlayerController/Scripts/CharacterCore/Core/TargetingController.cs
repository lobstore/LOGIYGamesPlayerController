using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class TargetingController
    {
        public Transform CurrentTarget { get; private set; }

        public bool HasTarget =>
            CurrentTarget != null;

        public void SetTarget(Transform target)
        {
            CurrentTarget = target;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
        }
    }
}
