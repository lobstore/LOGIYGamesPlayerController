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

        public JumpStateData groundJumpStateData = new();
        public JumpStateData wallJumpStateData = new();
        public JumpStateData rollStateData;
        public JumpStateData dashStateData;
        public JumpStateData slipStateData;
        public TimedMovementStateData backTurnStateData;
        public TimedMovementStateData turnStateData;
        public TimedMovementStateData landingStateData;
        public TimedMovementStateData ladderEnterStateData;
        public TimedMovementStateData ladderExitStateData;
        public TimedMovementStateData stoppingStateData;

        private LadderEnterState _ladderEnterState;
        private LadderExitState _ladderExitState;
        private LadderIdleState _ladderIdleState;
        private LadderUpState _ladderUpState;
        private LadderDownState _ladderDownState;
        private WallClimbMovementState _wallClimbState;
        private IdleMovementState _idleState;
        private SlideMovementState _slideState;
        private SlipMovementState _slipState;
        private BackTurnMovementState _backTurnState;
        private TurnMovementState _turnState;
        private LandingMovementState _landingState;
        private RunMovementState _runState;
        private WalkMovementState _walkState;
        private FallingMovementState _fallingState;
        private SprintMovementState _sprintState;
        private GroundJumpMovementState _groundJumpState;
        private WallClimbJumpMovementState _wallJumpState;
        private RollMovementState _rollState;
        private DashMovementState _dashState;
        private StopMovementState _stopState;

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
            _idleState = new IdleMovementState(character, idleStateData);
            _runState = new RunMovementState(character, runStateData);
            _walkState = new WalkMovementState(character, walkStateData);
            _sprintState = new SprintMovementState(character, sprintStateData);
            _fallingState = new FallingMovementState(character, fallingStateData);
            _groundJumpState = new GroundJumpMovementState(character, groundJumpStateData);
            _wallJumpState = new WallClimbJumpMovementState(character, wallJumpStateData);
            _rollState = new RollMovementState(character, rollStateData);
            _backTurnState = new BackTurnMovementState(character, backTurnStateData);
            _slideState = new SlideMovementState(character, slidingStateData);
            _landingState = new LandingMovementState(character, landingStateData);
            _dashState = new DashMovementState(character, dashStateData);
            _ladderEnterState = new LadderEnterState(character, ladderEnterStateData);
            _ladderExitState = new LadderExitState(character, ladderExitStateData);
            _ladderIdleState = new LadderIdleState(character, ladderIdleStateData);
            _ladderUpState = new LadderUpState(character, ladderClimbStateData);
            _ladderDownState = new LadderDownState(character, ladderClimbStateData);
            _wallClimbState = new WallClimbMovementState(character, wallClimbStateData);
            _stopState = new StopMovementState(character, stoppingStateData);
            _turnState = new TurnMovementState(character, turnStateData);
            _slipState = new SlipMovementState(character, slipStateData);


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
            character.AddAnyStateMachineTransition(_wallClimbState, () => character.Sensors.IsObstacleLegsFront 
            && character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") 
            && character.Input.MovementInput.y > 0
            && !IsOnLadder());
            character.AddAnyStateMachineTransition(_fallingState, () =>
            !character.Sensors.IsGrounded
            && !_groundJumpState.IsDurationTimerRunning
            && !_rollState.IsDurationTimerRunning
            && !IsOnLadder()
            && !character.Input.JumpPressed
            && !_wallClimbState.IsActiveState
            && !_wallJumpState.IsDurationTimerRunning
            );
            //NOTE: SEQUENCE IS IMPORTANT
            // ----- Idle State Transitions -----
            character.AddStateMachineTransition(_idleState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());
            character.AddStateMachineTransition(_idleState, _turnState, () => _turnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 45);
            character.AddStateMachineTransition(_idleState, _walkState, () => Input.GetKeyDown(KeyCode.Z));
            character.AddStateMachineTransition(_idleState, _runState, () => character.Input.MovementInput.magnitude > 0f);
            character.AddStateMachineTransition(_idleState, _ladderEnterState, () => _ladderEnterState.CanEnter() && character.Input.InteractPressed);

            // ----- Walk State Transitions -----
            character.AddStateMachineTransition(_walkState, _idleState, () => Input.GetKeyDown(KeyCode.Z));
            character.AddStateMachineTransition(_walkState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 120);
            // ----- Run State Transitions -----
            character.AddStateMachineTransition(_runState, _dashState, () => _dashState.CanEnter() && character.Input.SprintPressing);
            character.AddStateMachineTransition(_runState, _slipState, () => _slipState.CanEnter() && character.Input.CrouchPressed);
            character.AddStateMachineTransition(_slipState, _idleState, () => _slipState.IsDurationTimerElapsed && character.Input.MovementInput.magnitude==0);
            character.AddStateMachineTransition(_slipState, _runState, () => _slipState.IsDurationTimerElapsed && character.Input.MovementInput.magnitude > 0);
            character.AddStateMachineTransition(_runState, _groundJumpState, () => character.Input.JumpPressed && _groundJumpState.CanEnter());
            character.AddStateMachineTransition(_runState, _stopState, () => _stopState.CanEnter() && (character.Input.MovementInput.magnitude==0 || Mathf.Abs(character.DeltaYaw) > 120));

            character.AddStateMachineTransition(_stopState, _idleState, () => _stopState.IsDurationTimerElapsed);
            character.AddStateMachineTransition(_stopState, _backTurnState, () => _backTurnState.CanEnter() && Mathf.Abs(character.DeltaYaw) > 120);

            // ----- Sprint State Transitions -----
            character.AddStateMachineTransition(_sprintState, _idleState, () => character.Input.MovementInput.magnitude == 0f && character.Sensors.IsGrounded);
            character.AddStateMachineTransition(_sprintState, _runState, () => !character.Input.SprintPressing);
            character.AddStateMachineTransition(_sprintState, _stopState, () => _stopState.CanEnter() && (character.Input.MovementInput.magnitude == 0 || Mathf.Abs(character.DeltaYaw) > 120));
            character.AddStateMachineTransition(_sprintState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());

            // ----- Jump State Transitions -----
            character.AddStateMachineTransition(_groundJumpState, _runState, () => character.Sensors.IsGrounded && !_groundJumpState.IsDurationTimerRunning);

            // ----- Dash State Transitions -----

            character.AddStateMachineTransition(_dashState, _sprintState, () => character.Sensors.IsGrounded && !_dashState.IsDurationTimerRunning);


            // ----- Falling State Transitions -----

            character.AddStateMachineTransition(_fallingState, _landingState, () => character.Sensors.IsGrounded);
            character.AddStateMachineTransition(_fallingState, _groundJumpState, () => character.Input.JumpPressed && character.JumpCount < groundJumpStateData.MaxJumpCount && _groundJumpState.CanEnter());

            // ----- Landing State Transitions -----

            character.AddStateMachineTransition(_landingState, _runState, () => character.Sensors.IsGrounded && _landingState.IsDurationTimerElapsed && character.Input.MovementInput.magnitude>0);
            character.AddStateMachineTransition(_landingState, _idleState, () => character.Sensors.IsGrounded && _landingState.IsDurationTimerElapsed && character.Input.MovementInput.magnitude==0);

            // ----- Roll State Transitions -----
            character.AddStateMachineTransition(_rollState, _runState, () => character.IsGrounded && !_rollState.IsDurationTimerRunning &&character.Input.MovementInput.magnitude>0);
            character.AddStateMachineTransition(_rollState, _idleState, () => character.IsGrounded && !_rollState.IsDurationTimerRunning &&character.Input.MovementInput.magnitude==0);
            // TurnBack
            character.AddStateMachineTransition(_backTurnState, _runState, () => !_backTurnState.IsDurationTimerRunning);
            character.AddStateMachineTransition(_turnState, _idleState, () => !_turnState.IsDurationTimerRunning);


            //character.AddSubStateMachineTransition(_idleActionState, _unleashWeaponActionState, () => character.Input.AttackPressed);
            character.AddAnySubStateMachineTransition(_idleActionState, () => true);
            //character.AddSubStateMachineTransition(_unleashWeaponActionState, _readyActionState, () => true);
            //character.AddSubStateMachineTransition(_readyActionState, _leashWeaponActionState, () => character.Input.AttackPressed);
            //character.AddSubStateMachineTransition(_leashWeaponActionState, _idleActionState, () => true);
            //character.AddSubStateMachineTransition(_readyActionState, _throwItemActionState, () => character.Input.CrouchPressed);
            //character.AddSubStateMachineTransition(_throwItemActionState, _readyActionState, () => true);

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
            character.AddStateMachineTransition(_wallClimbState, _idleState, () => character.Input.InteractPressed  || !character.Sensors.IsObstacleLegsFront || !character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") || (character.IsGrounded && character.Input.MovementInput.y < 0));
            character.AddStateMachineTransition(_wallClimbState, _wallJumpState, () => _wallJumpState.CanEnter() && character.Input.JumpPressed );
            character.AddStateMachineTransition(_wallJumpState, _idleState, () => character.IsGrounded );
            character.AddStateMachineTransition(_fallingState, _wallClimbState, () => character.Sensors.IsObstacleLegsFront && character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") && character.Input.MovementInput.y>0);


        }




    }
}
