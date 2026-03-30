using UnityEngine;
using System.Collections;
using LOGIYGames.CharacterCore;

namespace LOGIYGames
{
    public class YAxisMovement : IMovementStrategy
    {
        private Character _character;
        public YAxisMovement(Character character)
        {
            _character = character;
        }
        public Vector3 GetMovementDirection()
        {
            return _character.transform.up * _character.Input.MovementInput.y;
        }
    }
}