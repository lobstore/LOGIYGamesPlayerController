using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraAlongRotation : IRotationStrategy
    {
        Character Character { get; set; }
        public CameraAlongRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            var targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            return targetRotation;
        }
    }

}

