using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class InputRelativeRotation : IRotationStrategy
    {
        private Character Character;

        public InputRelativeRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            if (Character.Input.MovementInput.magnitude > 0f)
            {
                Vector3 direction = new Vector3(Character.Input.MovementInput.x, 0, Character.Input.MovementInput.y);
                if (direction.magnitude > 0f)
                {
                    return Quaternion.LookRotation(direction.normalized, Vector3.up);
                }
            }

            float currentAngleY = Character.transform.eulerAngles.y;
            return Quaternion.Euler(0f, currentAngleY, 0f);
        }
    }
}
