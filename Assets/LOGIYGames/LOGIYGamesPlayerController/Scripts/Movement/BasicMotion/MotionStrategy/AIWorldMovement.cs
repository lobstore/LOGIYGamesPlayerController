using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Movement strategy that moves character in world space directions
    /// without camera influence. Used for AI characters.
    /// Input direction is treated as world-space direction.
    /// </summary>
    public class AIWorldMovement : IMovementStrategy
    {
        private Character _character;

        public AIWorldMovement(Character character)
        {
            _character = character;
        }

        public Vector3 GetMovementDirection()
        {
            // Input is already in world space from AI
            // Just flatten and normalize
            Vector3 direction = new Vector3(_character.MovementInput.x, 0, _character.MovementInput.y);
            return direction.normalized;
        }
    }
}
