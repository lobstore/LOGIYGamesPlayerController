using UnityEngine;
using System.Collections;
using LOGIYGames.CharacterCore;
using LOGIYGames.AI;

namespace LOGIYGames.Scripts.AI
{
	public class AIInputReader: ICharacterInputReader
	{
        AIBrain Driver;
        public AIInputReader(AIBrain Driver)
        {
            this.Driver = Driver;
        }

        public CharacterInput GetInput()
        {
            CharacterInput input = new();

            input.MovementInput = Driver.MovementInput;
            input.JumpPressed = Driver.JumpPressed;
            input.EvadePressed = Driver.EvadePressed;
            input.SprintPressing = Driver.SprintPressed;
            input.CrouchPressed = Driver.CrouchPressed;
            input.FocusPressed = Driver.FocusPressed;
            input.AttackPressed = false;
            input.InteractPressed = false;


            return input;
        }
    }
}