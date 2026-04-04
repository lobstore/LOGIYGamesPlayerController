using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderMovement : IMovementStrategy
    {
        Character Character;
        public LadderMovement(Character character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            return Character.transform.up * Character.Input.MovementInput.y;
        }
    }
}

