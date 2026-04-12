using UnityEngine;

namespace LOGIYGames
{
    public class NoneRotation : IRotationStrategy
    {
        Transform transform;

        public NoneRotation(Transform transform)
        {
            this.transform = transform;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(transform.forward);
        }
    }
}
