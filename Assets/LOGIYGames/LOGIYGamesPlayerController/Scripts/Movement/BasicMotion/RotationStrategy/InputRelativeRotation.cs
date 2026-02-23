using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Rotation strategy that rotates character to face movement direction
    /// without camera influence. Used for AI characters.
    /// Character rotates to face the direction it's moving.
    /// </summary>
    public class InputRelativeRotation : IRotationStrategy
    {
        private Character _character;

        public InputRelativeRotation(Character character)
        {
            _character = character;
        }

        public Quaternion GetRotation()
        {
            // If there's movement input, rotate to face movement direction
            if (_character.Input.MovementInput.magnitude > 0.01f)
            {
                Vector3 direction = new Vector3(_character.Input.MovementInput.x, 0, _character.Input.MovementInput.y);
                if (direction.magnitude > 0.01f)
                {
                    return Quaternion.LookRotation(direction.normalized, Vector3.up);
                }
            }

            // No movement - maintain current rotation
            float currentAngleY = _character.transform.eulerAngles.y;
            return Quaternion.Euler(0f, currentAngleY, 0f);
        }
    }
}
