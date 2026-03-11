using LOGIYGames.Movement;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MinimalMovementPreset", menuName = "MovementStateMachine/MovementStatesPreset/MinimalStatesSet")]
    public class MinimalMovementPreset : MovementStatesPresetBase
    {


        public MovementStateData idleStateData;
        public MovementStateData runStateData;
        public MovementStateData sprintStateData;
        public MovementStateData fallingStateData;
        public MovementStateData slidingStateData;
        public JumpStateData groundJumpStateData;
        public JumpStateData rollStateData;
        public TimedMovementStateData turnBackStateData;


        private IdleState _idleState;
        private SlideState _slideState;
        private BackTurnState _backTurnState;
        private RunState _runState;
        private FallingState _fallingState;
        private SprintState _sprintState;
        private JumpState _groundJumpState;
        private RollState _rollState;
		// Add AnimationModule to State subscribtion
        private void InitializeStates(MovementStateDriver MovementStateDriver)
        {
            _idleState = new IdleState(MovementStateDriver, idleStateData);
            _runState = new RunState(MovementStateDriver, runStateData);
            _sprintState = new SprintState(MovementStateDriver, sprintStateData);
            _fallingState = new FallingState(MovementStateDriver, fallingStateData);
            _groundJumpState = new JumpState(MovementStateDriver, groundJumpStateData);
            _rollState = new RollState(MovementStateDriver, rollStateData);
            _backTurnState = new BackTurnState(MovementStateDriver, turnBackStateData);
            _slideState = new SlideState(MovementStateDriver, slidingStateData);
        }
        private void ConfigureTransitions(MovementStateDriver MovementStateDriver)
        {
            MovementStateDriver.AddAnyTransition(_fallingState, () =>
            !MovementStateDriver.Sensors.IsGrounded
            && !_groundJumpState.IsDurationTimerRunning
            && !_rollState.IsDurationTimerRunning
            );
            MovementStateDriver.AddAnyTransition(_rollState, () =>
            MovementStateDriver.Sensors.IsGrounded
            && MovementStateDriver.Character.Input.EvadePressed
            );
            // ----- Idle State Transitions -----
            MovementStateDriver.AddTransition(_idleState, _groundJumpState, () => MovementStateDriver.Character.Input.JumpPressed && MovementStateDriver.Sensors.IsGrounded && _groundJumpState.CanEnter());

            // ----- Walk State Transitions -----
            MovementStateDriver.AddTransition(_idleState, _runState, () => MovementStateDriver.Character.Input.MovementInput.magnitude > 0f);
            MovementStateDriver.AddTransition(_runState, _groundJumpState, () => MovementStateDriver.Character.Input.JumpPressed && MovementStateDriver.Sensors.IsGrounded && _groundJumpState.CanEnter());

            // ----- Run State Transitions -----
            MovementStateDriver.AddTransition(_runState, _idleState, () => MovementStateDriver.Character.Input.MovementInput.magnitude == 0f && MovementStateDriver.Sensors.IsGrounded);
            MovementStateDriver.AddTransition(_runState, _sprintState, () => MovementStateDriver.Character.Input.SprintPressed && MovementStateDriver.Character.Input.MovementInput.magnitude > 0);
            MovementStateDriver.AddTransition(_runState, _groundJumpState, () => MovementStateDriver.Character.Input.JumpPressed && MovementStateDriver.Sensors.IsGrounded && _groundJumpState.CanEnter());
            MovementStateDriver.AddTransition(_runState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs( MovementStateDriver.Character.DeltaYaw)>120);
            MovementStateDriver.AddTransition(_runState, _slideState, () => MovementStateDriver.Sensors.GroundAngle > 25 && MovementStateDriver.Character.SpeedMultiplier > MovementStateDriver.Character.SpeedMultiplier*0.5f );

            // ----- Sprint State Transitions -----
            MovementStateDriver.AddTransition(_sprintState, _groundJumpState, () => MovementStateDriver.Character.Input.JumpPressed && MovementStateDriver.Sensors.IsGrounded && _groundJumpState.CanEnter());
            MovementStateDriver.AddTransition(_sprintState, _idleState, () => MovementStateDriver.Character.Input.MovementInput.magnitude == 0f && MovementStateDriver.Sensors.IsGrounded);
            MovementStateDriver.AddTransition(_sprintState, _runState, () => !MovementStateDriver.Character.Input.SprintPressed);

            // ----- Jump State Transitions -----
            MovementStateDriver.AddTransition(_groundJumpState, _runState, () => MovementStateDriver.Sensors.IsGrounded && !_groundJumpState.IsDurationTimerRunning);

            // ----- Falling State Transitions -----
            MovementStateDriver.AddTransition(_fallingState, _idleState, () => MovementStateDriver.Sensors.IsGrounded);
            // ----- Roll State Transitions -----
            MovementStateDriver.AddTransition(_rollState, _runState, () => MovementStateDriver.Sensors.IsGrounded&& !_rollState.IsDurationTimerRunning);
            // TurnBack
            MovementStateDriver.AddTransition(_backTurnState, _runState, () => !_backTurnState.IsDurationTimerRunning);
            MovementStateDriver.AddTransition(_slideState, _runState, () => MovementStateDriver.Sensors.GroundAngle <= 25|| MovementStateDriver.Character.Input.MovementInput.magnitude==0);

        }



        public override void Init(MovementStateDriver MovementStateDriver)
        {
            InitializeStates(MovementStateDriver);
            ConfigureTransitions(MovementStateDriver);
            MovementStateDriver.StateMachine.SetState(_idleState);
        }
    }
}
