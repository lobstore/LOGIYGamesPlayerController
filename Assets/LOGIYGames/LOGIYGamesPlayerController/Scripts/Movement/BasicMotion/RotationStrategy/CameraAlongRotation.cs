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
        private Character _character;
        
        public CameraAlongRotation(Character character)
        {
            _character = character;
        }
        
        public Quaternion GetRotation()
        {
            // Get camera - fallback to main camera if not available
            Camera cam = Camera.main;
            if (cam == null)
            {
                // No camera - just maintain current rotation
                float currentAngleY = _character.transform.eulerAngles.y;
                return Quaternion.Euler(0f, currentAngleY, 0f);
            }
            
            float targetAngleY = cam.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
            return targetRotation;
        }
    }
}
