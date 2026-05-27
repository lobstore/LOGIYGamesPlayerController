using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class WallRunMovement : IMovementStrategy
    {
        CharacterModule Character;

        public WallRunMovement(CharacterModule character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            Vector3 normal = Character.Sensors.IsObstacleLegsRight ? Character.Sensors.LegsRightHit.normal : Character.Sensors.LegsLeftHit.normal;

            Vector3 wallAlong = Vector3.Cross(normal, Character.transform.up).normalized;
            if ((Character.transform.forward - wallAlong).magnitude > (Character.transform.forward + wallAlong).magnitude)
            {
                wallAlong = -wallAlong;
            }

            return wallAlong;
        }
    }
}

