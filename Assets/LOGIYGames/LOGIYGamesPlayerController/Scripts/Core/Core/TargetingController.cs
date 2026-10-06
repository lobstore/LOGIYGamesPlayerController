using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class TargetingController
    {
        public Transform CurrentTarget;

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
