using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class TargetLockRotation : IRotationStrategy
    {
        private readonly Actor _character;

        public TargetLockRotation(Actor character)
        {
            _character = character;
        }

        public Quaternion GetRotation()
        {

            Vector3 direction =
                _character.TargetingController.CurrentTarget.position -
                _character.transform.position;

            direction.y = 0;

            return Quaternion.LookRotation(direction);
        }
    }
}

