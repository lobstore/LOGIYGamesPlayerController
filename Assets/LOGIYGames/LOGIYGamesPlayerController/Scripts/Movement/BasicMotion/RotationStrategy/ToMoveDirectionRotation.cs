using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMoveDirectionRotation : IRotationStrategy
    {
        CharacterModule Character;

        public ToMoveDirectionRotation(CharacterModule character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.TargetDirection);
        }
    }
}

