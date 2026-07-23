using System;
using UnityEngine;
using Alchemy.Inspector;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class MovementStateMachineViewer : MonoBehaviour
    {
        [SerializeField] Character Character;
        StateMachine StateMachine;
        [ReadOnly] public string _currentStateName;
        [ReadOnly] public string _lastTransition;
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
