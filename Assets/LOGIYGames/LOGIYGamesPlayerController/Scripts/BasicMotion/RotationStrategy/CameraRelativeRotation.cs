using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraRelativeRotation : IRotationStrategy
    {
        Character Character { get; set; }
        public CameraRelativeRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            if (Character.MovementInput.magnitude > 0f)
            {

                var targetAngleY = Mathf.Atan2(Character.MovementInput.x, Character.MovementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                return rotationY;

            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                return rotationY;
            }
        }
    }

}
