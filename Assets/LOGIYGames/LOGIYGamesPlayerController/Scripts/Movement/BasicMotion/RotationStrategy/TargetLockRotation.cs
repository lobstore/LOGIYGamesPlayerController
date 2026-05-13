using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class TargetLockRotation : IRotationStrategy
    {
        private readonly Character _character;

        public TargetLockRotation(Character character)
        {
            _character = character;
        }

        public Quaternion GetRotation()
        {
            if (!_character.Targeting.HasTarget)
            {
                float targetAngleY = Camera.main.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
                return targetRotation;
            }

            Vector3 direction =
                _character.Targeting.CurrentTarget.position -
                _character.transform.position;

            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f)
            {
                return _character.transform.rotation;
            }

            return Quaternion.LookRotation(direction);
        }
    }
}

