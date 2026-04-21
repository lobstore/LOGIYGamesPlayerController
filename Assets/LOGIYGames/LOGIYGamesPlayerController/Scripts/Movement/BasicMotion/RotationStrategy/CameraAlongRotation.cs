using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraAlongRotation : IRotationStrategy
    {
        public Quaternion GetRotation()
        {
            
            float targetAngleY = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
            return targetRotation;
        }
    }
}
