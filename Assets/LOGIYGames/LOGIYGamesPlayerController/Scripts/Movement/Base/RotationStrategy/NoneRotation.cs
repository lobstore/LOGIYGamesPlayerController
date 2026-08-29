using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class NoneRotation : IRotationStrategy
    {
        Actor Character;

        public NoneRotation(Actor character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.transform.forward);
        }
    }
}
