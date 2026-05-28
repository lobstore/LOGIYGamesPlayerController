using System;
using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public struct CharacterInput
    {
        public Vector2 MovementInput;
        public Vector3 LookForward;
        public Vector3 LookRight;

        public bool FocusPressed;
        public bool JumpPressed;
        public bool EvadePressed;
        public bool SprintPressing;
        public bool CrouchPressed;
        public bool InteractPressed;
        public bool AttackPressed;
        public bool HeavyAttackPressed;
    }
}
