using UnityEngine;
using System.Collections;
using LOGIYGames.CharacterCore;
using LOGIYGames.AI;

namespace LOGIYGames.Scripts.AI
{
    [RequireComponent(typeof(AIBrainStateDriver))]
	public class AIInputReader: MonoModuleBase, IInputReader
	{
        AIBrainStateDriver Driver;

        private void Start()
        {
            Driver = GetComponent<AIBrainStateDriver>();
        }
        public Vector2 MovementInput { get ; private set ; }
        public bool JumpPressed { get ; private set ; }
        public bool EvadePressed { get ; private set ; }
        public bool SprintPressed { get ; private set ; }
        public bool CrouchPressed { get ; private set ; }

        public bool FocusPressed {  get ; private set ; }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            MovementInput = Driver.MovementInput;
            JumpPressed = Driver.JumpPressed;
            EvadePressed = Driver.EvadePressed;
            SprintPressed = Driver.SprintPressed;
            CrouchPressed = Driver.CrouchPressed;
            FocusPressed = Driver.FocusPressed;
        }
    }
}