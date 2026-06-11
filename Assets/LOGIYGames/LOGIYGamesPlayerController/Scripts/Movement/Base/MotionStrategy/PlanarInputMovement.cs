using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class PlanarInputMovement : IMovementStrategy
    {

        CharacterModule Character;

        public PlanarInputMovement(CharacterModule character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            var fwd = Character.Input.LookForward;
            fwd.y = 0;
            var rght = Character.Input.LookRight;
            rght.y = 0;
            return rght.normalized * Character.Input.MovementInput.x + fwd.normalized * Character.Input.MovementInput.y;
        }
    }
}

