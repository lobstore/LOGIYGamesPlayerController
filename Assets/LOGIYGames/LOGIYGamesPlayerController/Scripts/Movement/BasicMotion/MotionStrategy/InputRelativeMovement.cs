using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class InputRelativeMovement : IMovementStrategy
    {
        CharacterModule Character;

        public InputRelativeMovement(CharacterModule character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            Vector3 direction = new Vector3(Character.Input.MovementInput.x, 0, Character.Input.MovementInput.y);
            return direction.normalized;
        }
    }
}
