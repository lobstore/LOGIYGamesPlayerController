using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMoveDirectionRotation : IRotationStrategy
    {
        Actor Character;

        public ToMoveDirectionRotation(Actor character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.RuntimeMovement.TargetDirection);
        }
    }
}

