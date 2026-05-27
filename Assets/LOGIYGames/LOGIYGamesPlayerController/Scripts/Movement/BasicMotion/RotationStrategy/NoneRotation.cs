using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class NoneRotation : IRotationStrategy
    {
        CharacterModule Character;

        public NoneRotation(CharacterModule character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.transform.forward);
        }
    }
}
