using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMovementDirectionRotation : IRotationStrategy
    {
        Actor Character;

        public ToMovementDirectionRotation(Actor character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            if (Character.Input.MovementInput.magnitude > 0f)
            {
                return Quaternion.LookRotation(Character.RuntimeMovement.TargetDirection);
            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                return Quaternion.Euler(0f, targetAngleY, 0f);
            }
        }
    }
}

