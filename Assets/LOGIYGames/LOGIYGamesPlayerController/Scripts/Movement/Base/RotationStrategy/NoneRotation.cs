using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class NoneRotation : IRotationStrategy
    {
        Character Character;

        public NoneRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.transform.forward);
        }
    }
}
