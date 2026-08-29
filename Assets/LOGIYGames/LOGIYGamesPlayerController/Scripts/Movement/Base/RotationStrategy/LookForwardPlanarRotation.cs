using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LookForwardPlanarRotation : IRotationStrategy
    {
        Actor character;

        public LookForwardPlanarRotation(Actor character)
        {
            this.character = character;
        }

        public Quaternion GetRotation()
        {
            Vector3 forward = character.Input.LookForward;

            // Убираем наклон по Y, чтобы персонаж не заваливался
            forward.y = 0f;


            forward.Normalize();

            return Quaternion.LookRotation(forward);
        }
    }
}
