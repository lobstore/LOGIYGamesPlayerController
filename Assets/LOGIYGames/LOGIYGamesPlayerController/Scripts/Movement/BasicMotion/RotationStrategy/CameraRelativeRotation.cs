using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraRelativeRotation : IRotationStrategy
    {
        Character Character;

        public CameraRelativeRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            
            if (Character.Input.MovementInput.magnitude > 0)
            {
                float targetAngleY = Mathf.Atan2(Character.Input.MovementInput.x, Character.Input.MovementInput.y) * Mathf.Rad2Deg;
                
                targetAngleY += Camera.main.transform.eulerAngles.y;
                
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
                return targetRotation;
            }
            else
            {
                float currentAngleY = Character.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, currentAngleY, 0f);
                return targetRotation;
            }
        }
    }
}
