using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LOGIYGames
{
    public class CharacterForwardMovement : IMovementStrategy
    {

        Actor Character;

        public CharacterForwardMovement(Actor character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            return Character.transform.forward;
        }
    }
}

