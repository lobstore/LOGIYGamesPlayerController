using UnityEngine;

namespace LOGIYGames
{
    public class NoneRotation : IRotationStrategy
    {
        Transform Transform;
        public NoneRotation(Transform transform)
        {
            Transform = transform;
        }
        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation( Transform.forward);
        }
    }
}
