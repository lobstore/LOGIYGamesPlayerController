using UnityEngine;

namespace LOGIYGames.AI
{
    public class InputProvider
    {
        // Movement input set by AI states
        public Vector2 MovementInput { get; set; }

        // Action flags set by AI states
        public bool JumpPressed { get; set; }
        public bool CrouchPressed { get; set; }
        public bool AttackPressed { get; set; }
        public bool SprintPressed { get; set; }
        public bool EvadePressed { get; set; }
        public bool BlockPressed { get; set; }

    }
}
