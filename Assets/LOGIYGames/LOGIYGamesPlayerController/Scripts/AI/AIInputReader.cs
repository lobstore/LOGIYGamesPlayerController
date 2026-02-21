using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// Receives movement and action commands from AI behavior states
    /// and provides them to MovementStateDriver similar to how InputReader
    /// provides player input.
    /// </summary>
    public class AIInputReader : MonoBehaviour
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

        /// <summary>
        /// Sets the movement input from AI behavior
        /// </summary>
        /// <param name="input">Movement direction as Vector2 (x, y)</param>
        public void SetMovementInput(Vector2 input)
        {
            MovementInput = input;
        }

        /// <summary>
        /// Sets the movement input from AI behavior using Vector3
        /// </summary>
        /// <param name="input">Movement direction as Vector3 (x, 0, z)</param>
        public void SetMovementInput(Vector3 input)
        {
            MovementInput = new Vector2(input.x, input.z);
        }

        /// <summary>
        /// Clears all input values (called when AI state exits)
        /// </summary>
        public void ClearAllInputs()
        {
            MovementInput = Vector2.zero;
            JumpPressed = false;
            CrouchPressed = false;
            AttackPressed = false;
            SprintPressed = false;
            EvadePressed = false;
            BlockPressed = false;
        }

        /// <summary>
        /// Triggers jump action
        /// </summary>
        public void PressJump()
        {
            JumpPressed = true;
        }

        /// <summary>
        /// Releases jump action
        /// </summary>
        public void ReleaseJump()
        {
            JumpPressed = false;
        }

        /// <summary>
        /// Triggers attack action
        /// </summary>
        public void PressAttack()
        {
            AttackPressed = true;
        }

        /// <summary>
        /// Releases attack action
        /// </summary>
        public void ReleaseAttack()
        {
            AttackPressed = false;
        }

        /// <summary>
        /// Triggers sprint action
        /// </summary>
        public void PressSprint()
        {
            SprintPressed = true;
        }

        /// <summary>
        /// Releases sprint action
        /// </summary>
        public void ReleaseSprint()
        {
            SprintPressed = false;
        }

        /// <summary>
        /// Triggers crouch action
        /// </summary>
        public void PressCrouch()
        {
            CrouchPressed = true;
        }

        /// <summary>
        /// Releases crouch action
        /// </summary>
        public void ReleaseCrouch()
        {
            CrouchPressed = false;
        }

        /// <summary>
        /// Triggers evade/roll action
        /// </summary>
        public void PressEvade()
        {
            EvadePressed = true;
        }

        /// <summary>
        /// Releases evade/roll action
        /// </summary>
        public void ReleaseEvade()
        {
            EvadePressed = false;
        }

        /// <summary>
        /// Triggers block action
        /// </summary>
        public void PressBlock()
        {
            BlockPressed = true;
        }

        /// <summary>
        /// Releases block action
        /// </summary>
        public void ReleaseBlock()
        {
            BlockPressed = false;
        }
    }
}
