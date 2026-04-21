using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMoveDirectionRotation : IRotationStrategy
    {
        Character Character;

        public ToMoveDirectionRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.targetDirection);
        }
    }
}

