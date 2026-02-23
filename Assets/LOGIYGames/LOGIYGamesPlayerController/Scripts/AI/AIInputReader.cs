using UnityEngine;
using System.Collections;
using LOGIYGames.CharacterCore;
using LOGIYGames.AI;

namespace LOGIYGames.Scripts.AI
{
	public class AIInputReader:IInputReader
	{

        public Vector2 MovementInput { get ; private set ; }
        public bool JumpPressed { get ; private set ; }
        public bool EvadePressed { get ; private set ; }
        public bool SprintPressed { get ; private set ; }
        public bool CrouchPressed { get ; private set ; }


        public void SetMovementInput(Vector2 movement)
        {
            MovementInput = movement;
        }

        public void SetJumpPressed(bool jumpPressed)
        {
            JumpPressed = jumpPressed;
        }

        public void SetEvadePressed(bool evadePressed)
        {
            EvadePressed = evadePressed;
        }

        public void SetSprintPressed(bool sprintPressed) 
        {
            SprintPressed = sprintPressed;
        }

        public void SetCrouchPressed(bool crouchPressed)
        {
            CrouchPressed = crouchPressed;
        }
    }
}