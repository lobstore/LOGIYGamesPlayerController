using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "FullMovementPreset", menuName = "MovementStateMachine/MovementStatesPreset/FullMovementPreset")]
    public class FullMovementPreset : MovementStatesPresetBase
    {


        public MovementStateData idleStateData;
        public MovementStateData runStateData;
        public MovementStateData walkStateData;
        public MovementStateData sprintStateData;
        public MovementStateData fallingStateData;
        public MovementStateData slidingStateData;
        public MovementStateData ladderIdleStateData;
        public MovementStateData ladderClimbStateData;
        public MovementStateData wallClimbStateData;

        public JumpStateData groundJumpStateData;
        public JumpStateData rollStateData;
        public JumpStateData dashStateData;
        public TimedMovementStateData turnBackStateData;
        public TimedMovementStateData landingStateData;
        public TimedMovementStateData ladderEnterStateData;
        public TimedMovementStateData ladderExitStateData;

        private LadderEnterState _ladderEnterState;
        private LadderExitState _ladderExitState;
        private LadderIdleState _ladderIdleState;
        private LadderUpState _ladderUpState;
        private LadderDownState _ladderDownState;
        private WallClimbState _wallClimbState;
        private IdleState _idleState;
        private SlideState _slideState;
        private TurnState _backTurnState;
        private LandingState _landingState;
        private RunState _runState;
        private WalkState _walkState;
        private FallingState _fallingState;
        private SprintState _sprintState;
        private JumpState _groundJumpState;
        private RollState _rollState;
        private DashState _dashState;

        private IdleActionState _idleActionState;
        private CombatActionState _readyActionState;
        private UnleashWeaponActionState _unleashWeaponActionState;
        private LeashWeaponActionState _leashWeaponActionState;
        private ThrowItemActionState _throwItemActionState;

        public override void Init(Character character)
        {
            InitializeStates(character);
            ConfigureTransitions(character);
            character.MovementStateMachine.SetState(_idleState);
            character.ActionStateMachine.SetState(_idleActionState);
        }

        // Add AnimationModule to State subscribtion
        private void InitializeStates(Character character)
        {
            _idleState = new IdleState(character, idleStateData);
            _runState = new RunState(character, runStateData);
            _walkState = new WalkState(character, walkStateData);
            _sprintState = new SprintState(character, sprintStateData);
            _fallingState = new FallingState(character, fallingStateData);
            _groundJumpState = new JumpState(character, groundJumpStateData);
            _rollState = new RollState(character, rollStateData);
            _backTurnState = new TurnState(character, turnBackStateData);
            _slideState = new SlideState(character, slidingStateData);
            _landingState = new LandingState(character, landingStateData);
            _dashState = new DashState(character, dashStateData);
            _ladderEnterState = new LadderEnterState(character, ladderEnterStateData);
            _ladderExitState = new LadderExitState(character, ladderExitStateData);
            _ladderIdleState = new LadderIdleState(character, ladderIdleStateData);
            _ladderUpState = new LadderUpState(character, ladderClimbStateData);
            _ladderDownState = new LadderDownState(character, ladderClimbStateData);
            _wallClimbState = new WallClimbState(character, wallClimbStateData);

            _idleActionState = new IdleActionState(character);
            _readyActionState = new CombatActionState(character);
            _unleashWeaponActionState = new UnleashWeaponActionState(character);
            _leashWeaponActionState = new LeashWeaponActionState(character);
            _throwItemActionState = new ThrowItemActionState(character);
        }

        private bool IsOnLadder()
        {
            return _ladderDownState.IsActiveState
                || _ladderIdleState.IsActiveState
                || _ladderEnterState.IsActiveState
                || _ladderExitState.IsActiveState
                || _ladderUpState.IsActiveState;
        }
        private void ConfigureTransitions(Character character)
        {
            character.AddAnyStateMachineTransition(_rollState, () => character.Input.EvadePressed
            && !_groundJumpState.IsDurationTimerRunning && character.IsGrounded
            && !IsOnLadder());
            character.AddAnyStateMachineTransition(_fallingState, () =>
            !character.Sensors.IsGrounded
            && !_groundJumpState.IsDurationTimerRunning
            && !_rollState.IsDurationTimerRunning
            && !IsOnLadder()
            && !character.Input.JumpPressed
            && !_wallClimbState.IsActiveState
            );
            //NOTE: SEQUENCE IS IMPORTANT
            // ----- Idle State Transitions -----
            character.AddStateMachineTransition(_idleState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());
            character.AddStateMachineTransition(_idleState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 120);
            character.AddStateMachineTransition(_idleState, _walkState, () => Input.GetKeyDown(KeyCode.Z));
            character.AddStateMachineTransition(_idleState, _runState, () => character.Input.MovementInput.magnitude > 0f);
            character.AddStateMachineTransition(_idleState, _ladderEnterState, () => _ladderEnterState.CanEnter() && character.Input.InteractPressed);

            // ----- Walk State Transitions -----
            character.AddStateMachineTransition(_walkState, _idleState, () => Input.GetKeyDown(KeyCode.Z));
            character.AddStateMachineTransition(_walkState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 120);
            // ----- Run State Transitions -----
            character.AddStateMachineTransition(_runState, _idleState, () => character.Input.MovementInput.magnitude == 0f && character.Sensors.IsGrounded);
            //character.AddStateMachineTransition(_runState, _sprintState, () => character.Input.SprintPressing && character.Input.MovementInput.magnitude > 0);
            character.AddStateMachineTransition(_runState, _dashState, () => Input.GetKeyDown(KeyCode.LeftShift));
            character.AddStateMachineTransition(_runState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 120);
            //character.AddStateMachineTransition(_runState, _slideState, () => character.Sensors.GroundAngle > 25 && character.SpeedMultiplier > character.SpeedMultiplier * 0.5f);
            character.AddStateMachineTransition(_runState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());
            character.AddStateMachineTransition(_runState, _wallClimbState, () => character.Sensors.IsObstacleLegsFront && character.Sensors.LegsFrontHit.collider.CompareTag("Climbable"));

            // ----- Sprint State Transitions -----
            character.AddStateMachineTransition(_sprintState, _idleState, () => character.Input.MovementInput.magnitude == 0f && character.Sensors.IsGrounded);
            character.AddStateMachineTransition(_sprintState, _runState, () => !character.Input.SprintPressing);
            character.AddStateMachineTransition(_sprintState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());

            // ----- Jump State Transitions -----
            character.AddStateMachineTransition(_groundJumpState, _runState, () => character.Sensors.IsGrounded && !_groundJumpState.IsDurationTimerRunning);

            // ----- Dash State Transitions -----

            character.AddStateMachineTransition(_dashState, _sprintState, () => character.Sensors.IsGrounded && !_dashState.IsDurationTimerRunning);


            // ----- Falling State Transitions -----

            character.AddStateMachineTransition(_fallingState, _landingState, () => character.Sensors.IsGrounded);
            character.AddStateMachineTransition(_fallingState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());

            // ----- Landing State Transitions -----

            character.AddStateMachineTransition(_landingState, _runState, () => character.Sensors.IsGrounded && _landingState.IsDurationTimerElapsed);

            // ----- Roll State Transitions -----
            character.AddStateMachineTransition(_rollState, _runState, () => character.IsGrounded && !_rollState.IsDurationTimerRunning);
            // TurnBack
            character.AddStateMachineTransition(_backTurnState, _runState, () => !_backTurnState.IsDurationTimerRunning);
            character.AddStateMachineTransition(_backTurnState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 120);
            character.AddStateMachineTransition(_slideState, _runState, () => character.Sensors.GroundAngle <= 25 || character.Input.MovementInput.magnitude == 0);

            character.AddSubStateMachineTransition(_idleActionState, _unleashWeaponActionState, () => character.Input.AttackPressed);
            character.AddSubStateMachineTransition(_unleashWeaponActionState, _readyActionState, () => true);
            character.AddSubStateMachineTransition(_readyActionState, _leashWeaponActionState, () => character.Input.AttackPressed);
            character.AddSubStateMachineTransition(_leashWeaponActionState, _idleActionState, () => true);
            character.AddSubStateMachineTransition(_readyActionState, _throwItemActionState, () => character.Input.CrouchPressed);
            character.AddSubStateMachineTransition(_throwItemActionState, _readyActionState, () => true);

            // ----- From Ladder State Transitions -----
            character.AddStateMachineTransition(_ladderEnterState, _ladderIdleState, () => _ladderEnterState.IsDurationTimerElapsed);
            character.AddStateMachineTransition(_ladderIdleState, _ladderExitState, () => _ladderExitState.CanEnter() && character.Input.InteractPressed);
            character.AddStateMachineTransition(_ladderIdleState, _ladderUpState, () => character.Input.MovementInput.y > 0);
            character.AddStateMachineTransition(_ladderUpState, _ladderIdleState, () => character.Input.MovementInput.y <= 0 );
            character.AddStateMachineTransition(_ladderIdleState, _ladderDownState, () => character.Input.MovementInput.y < 0);
            character.AddStateMachineTransition(_ladderDownState, _ladderIdleState, () => character.Input.MovementInput.y >= 0);
            character.AddStateMachineTransition(_ladderDownState, _ladderExitState, () => _ladderExitState.CanEnter());
            character.AddStateMachineTransition(_ladderUpState, _ladderExitState, () =>  _ladderExitState.CanEnter());
            character.AddStateMachineTransition(_ladderExitState, _idleState, () => _ladderExitState.IsDurationTimerElapsed);
            // ----- From Wall Climb State Transitions -----
            character.AddStateMachineTransition(_wallClimbState, _idleState, () => character.Input.JumpPressed  || !character.Sensors.IsObstacleLegsFront || !character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") || (character.IsGrounded && character.Input.MovementInput.y < 0));


        }




    }
}
