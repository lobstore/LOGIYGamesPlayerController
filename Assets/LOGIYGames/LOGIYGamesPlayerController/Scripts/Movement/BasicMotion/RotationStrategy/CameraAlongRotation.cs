using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Rotation strategy that rotates character to face the same direction as the camera.
    /// Character rotates to match camera's Y rotation regardless of movement input.
    /// </summary>
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
