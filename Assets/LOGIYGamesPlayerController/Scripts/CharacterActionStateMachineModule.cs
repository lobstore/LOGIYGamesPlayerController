using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public class CharacterActionStateMachineModule : NetworkModuleBase
    {
        StateMachine StateMachine;
        private void Awake()
        {
            StateMachine = new StateMachine();

        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            StateMachine.FixedUpdate();
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            StateMachine.Update();
        }
    }
}