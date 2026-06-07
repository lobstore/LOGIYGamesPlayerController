using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class WallClimbMovement : IMovementStrategy
    {
        SensorsModule Sensors;
        CharacterModule Character;

        public WallClimbMovement(SensorsModule sensors, CharacterModule character)
        {
            Sensors = sensors;
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            Vector3 normal = Sensors.LegsFrontHit.normal;

            Vector3 wallAlongUp = Vector3.ProjectOnPlane(Character.transform.up, normal).normalized;
            Vector3 wallAlongRight = Vector3.ProjectOnPlane(Character.transform.right, normal).normalized;

            Vector3 horizontalDir = Vector3.zero;
            Vector3 verticalDir = Vector3.zero;

            horizontalDir = wallAlongRight * Character.Input.MovementInput.x;

            verticalDir = wallAlongUp * Character.Input.MovementInput.y;
            return horizontalDir + verticalDir;
        }
    }
}

