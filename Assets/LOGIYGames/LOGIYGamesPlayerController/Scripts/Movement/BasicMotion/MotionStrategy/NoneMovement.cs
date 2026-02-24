using UnityEngine;

namespace LOGIYGames
{
    public class NoneMovement : IMovementStrategy
    {
        public Vector3 GetMovementDirection()
        {
            return Vector3.zero;
        }
    }
}
