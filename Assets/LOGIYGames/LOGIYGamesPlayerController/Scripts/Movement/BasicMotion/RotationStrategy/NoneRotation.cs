using UnityEngine;

namespace LOGIYGames
{
    public class NoneRotation : IRotationStrategy
    {
        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Vector3.forward);
        }
    }
}
