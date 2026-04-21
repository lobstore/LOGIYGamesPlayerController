using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class SwimRotation : IRotationStrategy
    {
        Character Character;

        public SwimRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            if (Character.Input.MovementInput.magnitude > 0f)
            {
                return Quaternion.LookRotation(Character.Velocity);
            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                return Quaternion.Euler(0f, targetAngleY, 0f);
            }
        }
    }
}

