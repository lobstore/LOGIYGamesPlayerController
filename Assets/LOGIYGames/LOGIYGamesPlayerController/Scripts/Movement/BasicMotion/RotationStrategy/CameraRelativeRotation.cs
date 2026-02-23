using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Rotation strategy that rotates character relative to camera direction based on movement input.
    /// When there's no movement input, the character maintains its current rotation.
    /// </summary>
    public class CameraRelativeRotation : IRotationStrategy
    {
        private Character _character;
        
        public CameraRelativeRotation(Character character)
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
            
            if (_character.Input.MovementInput.magnitude > 0.01f)
            {
                // Calculate target angle from movement input
                // Atan2 returns angle in radians, convert to degrees
                float targetAngleY = Mathf.Atan2(_character.Input.MovementInput.x, _character.Input.MovementInput.y) * Mathf.Rad2Deg;
                
                // Add camera's Y rotation to make it camera-relative
                targetAngleY += cam.transform.eulerAngles.y;
                
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
                return targetRotation;
            }
            else
            {
                // No input - maintain current rotation
                // Preserve current Y rotation only (flatten any X/Z rotation)
                float currentAngleY = _character.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, currentAngleY, 0f);
                return targetRotation;
            }
        }
    }
}
