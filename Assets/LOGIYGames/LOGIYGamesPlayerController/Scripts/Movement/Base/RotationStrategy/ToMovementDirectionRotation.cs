using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMovementDirectionRotation : IRotationStrategy
    {
        Character Character;

        public ToMovementDirectionRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            if (Character.Input.MovementInput.magnitude > 0f)
            {
                return Quaternion.LookRotation(Character.RuntimeMovement.TargetVelocity);
            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                return Quaternion.Euler(0f, targetAngleY, 0f);
            }
        }
    }
}

