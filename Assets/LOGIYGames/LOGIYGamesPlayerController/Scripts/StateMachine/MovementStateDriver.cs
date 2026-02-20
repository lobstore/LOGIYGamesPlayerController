using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Drives the character movement state machine with support for timed transitions
    /// </summary>
    public class MovementStateDriver : MonoBehaviour
    {
        public Character Character;
        public SensorsModule Sensors;
        public InputReader InputReader;
        public StateMachine StateMachine => _stateMachine;

        [Header("State Machine Configuration")]
        [SerializeField] private StatesDataSO statesDataSO;

        private StateMachine _stateMachine;

        #region States

        private IdleState _idleState;
        private RunState _runState;
        private SprintState _sprintState;
        private FallingState _fallingState;
        private LandingState _landingState;
        private JumpState _groundJumpState;
        private StopState _stopState;
        private CrouchState _crouchState;
        private RollState _rollState;

        #endregion

        #region Debug

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        private string _currentStateName;
        private string _lastTransition;

        #endregion

        private void Start()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine();

            // Initialize all states
            InitializeStates();

            // Set initial state
            _stateMachine.SetState(_idleState);
        }

        private void InitializeStates()
        {
            _idleState = new IdleState(this, statesDataSO.IdleStateData);
            _runState = new RunState(this, statesDataSO.RunStateData);
            _sprintState = new SprintState(this, statesDataSO.SprintStateData);
            _fallingState = new FallingState(this, statesDataSO.FallingStateData);
            _landingState = new LandingState(this, statesDataSO.LandingStateData);
            _groundJumpState = new JumpState(this, statesDataSO.GroundJumpStateData);
            _stopState = new StopState(this, statesDataSO.StoppingStateData);
            _crouchState = new CrouchState(this, statesDataSO.CrouchStateData);
            _rollState = new RollState(this, statesDataSO.RollStateData);
        }

        private void Update()
        {
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
    }
}
