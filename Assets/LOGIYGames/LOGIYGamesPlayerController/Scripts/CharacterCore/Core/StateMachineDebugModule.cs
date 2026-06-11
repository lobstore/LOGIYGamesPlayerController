using System;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class StateMachineDebugModule
    {
        StateMachine StateMachine;
        private string _currentStateName;
        private string _lastTransition;
        public StateMachineDebugModule(StateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        public void Update()
        {
            _currentStateName = StateMachine.CurrentNode.State.ToString();
            _lastTransition = StateMachine.LastTransition;
        }
    }
}
