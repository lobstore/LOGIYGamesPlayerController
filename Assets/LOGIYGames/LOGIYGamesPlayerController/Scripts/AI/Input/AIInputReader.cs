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
        public Vector2 MovementInput => Driver.MovementInput;
        public bool JumpPressed=> Driver.JumpPressed;
        public bool EvadePressed => Driver.EvadePressed;
        public bool SprintPressing => Driver.SprintPressed;
        public bool CrouchPressed => Driver.CrouchPressed;

        public bool FocusPressed => Driver.FocusPressed;

        public bool AttackPressed => false;

        public bool InteractPressed => false;
    }
}