using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class Input360LookMovement : IMovementStrategy
    {
        Character Character;

        public Input360LookMovement(Character character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            Vector3 movement = new Vector3(Character.Input.MovementInput.x, 0, Character.Input.MovementInput.y);

            Vector3 forward = Character.Input.LookForward;
            Vector3 right = Character.Input.LookRight;

            forward.Normalize();
            right.Normalize();

            Vector3 move = (right * movement.x) + (forward * movement.z);
            return move.normalized;
        }
    }

}

