using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraRelativeMovement : IMovementStrategy
    {
        Character Character { get; set; }
        public CameraRelativeMovement(Character character)
        {
            Character = character;
        }
        public Vector3 GetMovementDirection()
        {

                var fwd = Camera.main.transform.forward;
                fwd.y = 0;
                var rght = Camera.main.transform.right;
                rght.y = 0;
                return rght.normalized * Character.Input.MovementInput.x + fwd.normalized * Character.Input.MovementInput.y;

        }
    }

}

