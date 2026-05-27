using UnityEngine;
using System.Collections;
using LOGIYGames.CharacterCore;

namespace LOGIYGames
{
    public class YAxisMovement : IMovementStrategy
    {
        private CharacterModule _character;
        public YAxisMovement(CharacterModule character)
        {
            _character = character;
        }
        public Vector3 GetMovementDirection()
        {
            return _character.transform.up * _character.Input.MovementInput.y;
        }
    }
}