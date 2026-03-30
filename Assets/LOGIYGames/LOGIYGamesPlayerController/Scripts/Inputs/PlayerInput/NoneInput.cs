using UnityEngine;



namespace LOGIYGames.CharacterCore
{
    public class NoneInput : IMovementInputReader
    {
        public Vector2 MovementInput => Vector3.zero;

        public bool FocusPressed => false;

        public bool JumpPressed => false;

        public bool EvadePressed => false;

        public bool SprintPressing => false;

        public bool CrouchPressed => false;

        public bool AttackPressed => false;

    }
}
