using LOGIYGames.Animation;
using LOGIYGames.CharacterCore;
using System;
using UnityEngine;

namespace LOGIYGames.Movement
{
    /// <summary>
    /// Drives the character movement state machine with support for timed transitions
    /// </summary>
    public class MovementStateDriver : MonoBehaviour
    {
        public Character Character;
        public SensorsModule Sensors;
        public StateMachine StateMachine => _stateMachine;

        [Header("State Machine Configuration")]
        public MovementStatesPresetBase movementPreset;
        private StateMachine _stateMachine;
        [SerializeField] private ControllerWrapperBase controller;
        [field: SerializeField] public CharacterAnimationsModule Animations {  get; private set; }


        #region Debug

        private string _currentStateName;
        private string _lastTransition;

        #endregion

        private void Awake()
        {

            Character.CController = controller;
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine();
            if (movementPreset != null)
            {
                movementPreset.Init(this);

            }
            else
            {
                Debug.LogError("No MovementPreset provided");
            }
        }

        private void Update()
        {
            _currentStateName = _stateMachine.CurrentNode.State.ToString();
            _lastTransition = _stateMachine.LastTransition;
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        private void LateUpdate()
        {
            _stateMachine.LateUpdate();
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            _stateMachine.AddAnyTransition(to, new FuncPredicate(condition));
        }
    }
}
