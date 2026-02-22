using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraAlongMovement : IMovementStrategy
    {
        Character Character { get; set; }
        public CameraAlongMovement(Character character)
        {
            Character = character;
        }
        public Vector3 GetMovementDirection()
        {
            var fwd = Camera.main.transform.forward;
            fwd.y = 0;
            var rght = Camera.main.transform.right;
            rght.y = 0;
            return rght.normalized * Character.InputProvider.MovementInput.x + fwd.normalized * Character.InputProvider.MovementInput.y;
        }
    }

}

