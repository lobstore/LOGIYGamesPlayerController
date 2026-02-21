using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
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
        [SerializeField] private StatesDataSO statesDataSO;

        private StateMachine _stateMachine;

        #region States

        private IdleState _idleState;
        private WalkState _walkState;
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

            // Configure all transitions
            ConfigureTransitions();
            _stateMachine.SetState(_idleState);
        }

        private void InitializeStates()
        {
            _idleState = new IdleState(this, statesDataSO.IdleStateData);
            _walkState = new WalkState(this, statesDataSO.WalkStateData);
            _runState = new RunState(this, statesDataSO.RunStateData);
            _sprintState = new SprintState(this, statesDataSO.SprintStateData);
            _fallingState = new FallingState(this, statesDataSO.FallingStateData);
            _landingState = new LandingState(this, statesDataSO.LandingStateData);
            _groundJumpState = new JumpState(this, statesDataSO.GroundJumpStateData);
            _stopState = new StopState(this, statesDataSO.StoppingStateData);
            _crouchState = new CrouchState(this, statesDataSO.CrouchStateData);
            _rollState = new RollState(this, statesDataSO.RollStateData);
        }

        /// <summary>
        /// Configures all state transitions based on the transition table
        /// Transitions are organized by source state to avoid duplicates
        /// </summary>
        private void ConfigureTransitions()
        {
            // ============================================
            // TRANSITION TABLE
            // ============================================
            // From State     | To State      | Condition
            // --------------------------------------------
            // Idle           | Walk          | Movement input > 0
            // Idle           | Jump          | Jump pressed & grounded
            // Idle           | Crouch        | Crouch pressed
            // Idle           | Roll          | Evade pressed
            // Idle           | Falling       | Not grounded
            // --------------------------------------------
            // Walk           | Idle          | Movement input = 0
            // Walk           | Run           | Movement input > threshold & not sprinting
            // Walk           | Jump          | Jump pressed & grounded
            // Walk           | Crouch        | Crouch pressed
            // Walk           | Roll          | Evade pressed
            // Walk           | Falling       | Not grounded
            // --------------------------------------------
            // Run            | Idle          | Movement input = 0
            // Run            | Walk          | Movement input < threshold
            // Run            | Sprint        | Sprint pressed
            // Run            | Jump          | Jump pressed & grounded
            // Run            | Crouch        | Crouch pressed
            // Run            | Roll          | Evade pressed
            // Run            | Falling       | Not grounded
            // --------------------------------------------
            // Sprint         | Idle          | Movement input = 0
            // Sprint         | Run           | Sprint released
            // Sprint         | Jump          | Jump pressed & grounded
            // Sprint         | Crouch        | Crouch pressed
            // Sprint         | Roll          | Evade pressed
            // Sprint         | Falling       | Not grounded
            // --------------------------------------------
            // Crouch         | Idle          | Crouch released & grounded
            // Crouch         | Walk          | Movement input & crouch released
            // Crouch         | Roll          | Evade pressed
            // Crouch         | Falling       | Not grounded
            // --------------------------------------------
            // Jump           | Falling       | Jump duration elapsed & not grounded
            // Jump           | Landing       | Jump duration elapsed & grounded
            // --------------------------------------------
            // Falling        | Landing       | Grounded
            // --------------------------------------------
            // Landing        | Idle          | Landing duration elapsed & no input
            // Landing        | Walk          | Landing duration elapsed & movement input
            // Landing        | Roll          | Evade pressed
            // --------------------------------------------
            // Roll           | Idle          | Roll duration elapsed & no input
            // Roll           | Walk          | Roll duration elapsed & movement input
            // Roll           | Falling       | Roll duration elapsed & not grounded
            // =============================================
            AddAnyTransition(_fallingState, () => 
            !Sensors.IsGrounded 
            && !_groundJumpState.IsDurationTimerRunning
            && !_rollState.IsDurationTimerRunning
            );
            // ----- Idle State Transitions -----
            AddTransition(_idleState, _walkState, () => HasMovementInput() && IsGrounded());
            AddTransition(_idleState, _groundJumpState, () => Character.JumpPressed && IsGrounded());
            AddTransition(_idleState, _crouchState, () => Character.CrouchPressed);
            AddTransition(_idleState, _rollState, () => Character.EvadePressed);

            // ----- Walk State Transitions -----
            AddTransition(_walkState, _idleState, () => !HasMovementInput() && IsGrounded());
            AddTransition(_walkState, _runState, () => HasStrongMovementInput() && !IsSprinting());
            AddTransition(_walkState, _groundJumpState, () => Character.JumpPressed && IsGrounded());
            AddTransition(_walkState, _crouchState, () => Character.CrouchPressed);
            AddTransition(_walkState, _rollState, () => Character.EvadePressed);

            // ----- Run State Transitions -----
            AddTransition(_runState, _idleState, () => !HasMovementInput() && IsGrounded());
            AddTransition(_runState, _walkState, () => HasMovementInput() && !HasStrongMovementInput());
            AddTransition(_runState, _sprintState, () => Character.SprintPressed && HasStrongMovementInput());
            AddTransition(_runState, _groundJumpState, () => Character.JumpPressed && IsGrounded());
            AddTransition(_runState, _crouchState, () => Character.CrouchPressed);
            AddTransition(_runState, _rollState, () => Character.EvadePressed);

            // ----- Sprint State Transitions -----
            AddTransition(_sprintState, _idleState, () => !HasMovementInput() && IsGrounded());
            AddTransition(_sprintState, _runState, () => !Character.SprintPressed);
            AddTransition(_sprintState, _groundJumpState, () => Character.JumpPressed && IsGrounded());
            AddTransition(_sprintState, _crouchState, () => Character.CrouchPressed);
            AddTransition(_sprintState, _rollState, () => Character.EvadePressed);

            // ----- Crouch State Transitions -----
            AddTransition(_crouchState, _idleState, () => !Character.CrouchPressed && IsGrounded() && !HasMovementInput());
            AddTransition(_crouchState, _walkState, () => !Character.CrouchPressed && HasMovementInput());
            AddTransition(_crouchState, _rollState, () => Character.EvadePressed);

            // ----- Jump State Transitions -----
            AddTransition(_groundJumpState, _landingState, () =>IsGrounded() && _groundJumpState.IsDurationTimerElapsed);

            // ----- Falling State Transitions -----
            AddTransition(_fallingState, _landingState, () => IsGrounded());

            // ----- Landing State Transitions -----
            AddTransition(_landingState, _idleState, () => _landingState.IsDurationTimerElapsed && !HasMovementInput());
            AddTransition(_landingState, _walkState, () => _landingState.IsDurationTimerElapsed && HasMovementInput());
            AddTransition(_landingState, _rollState, () => Character.EvadePressed);

            // ----- Roll State Transitions -----
            AddTransition(_rollState, _idleState, () => _rollState.IsDurationTimerElapsed && !HasMovementInput() && IsGrounded());
            AddTransition(_rollState, _walkState, () => _rollState.IsDurationTimerElapsed && HasMovementInput());

        }

        /// <summary>
        /// Helper method to add transition with inline predicate
        /// </summary>
        private void AddTransition(IState from, IState to, Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }

        private void AddAnyTransition(IState to, Func<bool> condition)
        {
            _stateMachine.AddAnyTransition(to, new FuncPredicate(condition));
        }

        #region Condition Helpers

        private bool HasMovementInput() => Character.MovementInput.magnitude > 0.1f;

        private bool HasStrongMovementInput() => Character.MovementInput.magnitude > 0.6f;

        private bool IsGrounded() => Sensors.IsGrounded;

        private bool IsSprinting() => Character.SprintPressed;

        #endregion

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
    }
}
