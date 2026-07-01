using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class MovementStateMachineViewer : MonoBehaviour
    {
        [SerializeField] Character Character;
        StateMachine StateMachine;
        private string _currentStateName;
        private string _lastTransition;
        private void Start()
        {
            StateMachine = Character.MovementStateMachine;
        }
        private void Update()
        {
            _currentStateName = StateMachine.CurrentNode.State.ToString();
            _lastTransition = StateMachine.LastTransition;
        }
    }
}
