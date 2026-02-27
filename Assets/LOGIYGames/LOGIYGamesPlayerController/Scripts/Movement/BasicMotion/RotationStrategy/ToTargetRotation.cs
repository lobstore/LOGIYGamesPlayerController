using UnityEngine;

namespace LOGIYGames
{
    public class ToTargetRotation : IRotationStrategy
    {
        Transform Target;
        public ToTargetRotation(Transform target)
        {
            Target = target;
        }
        public Quaternion GetRotation()
        {

            Vector3 targetDirection = Target.position - Target.transform.position;
            targetDirection.y = 0f;
            return Quaternion.LookRotation(targetDirection);
        }
    }
}
