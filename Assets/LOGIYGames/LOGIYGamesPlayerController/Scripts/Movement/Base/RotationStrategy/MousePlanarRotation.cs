using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class MousePlanarRotation : IRotationStrategy
    {
        private readonly Actor character;
        public MousePlanarRotation(Actor character)
        {
            this.character = character;
        }

        public Quaternion GetRotation()
        {
            Vector2 lookInput =
                CameraManager.Instance.CameraInput.LookInput;

            return Quaternion.Euler(0f, character.transform.eulerAngles.y + lookInput.x, 0f);
        }
    }
}
