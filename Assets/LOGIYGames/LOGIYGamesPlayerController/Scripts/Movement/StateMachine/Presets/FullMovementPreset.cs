using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "FullMovementPreset", menuName = "MovementStateMachine/MovementStatesPreset/FullMovementPreset")]
    public class FullMovementPreset : MovementStatesPresetBase
    {

        [Header("GroundLocomotion")]
        public MovementStateData idleMovementStateData;
        public MovementStateData runMovementStateData;
        public MovementStateData walkMovementStateData;
        public MovementStateData sprintMovementStateData;
        public MovementStateData slidingMovementStateData;
        [Header("AirLocomotion")]
        public MovementStateData flyMovementStateData;
        public MovementStateData fallingMovementStateData;
        [Header("Waterlocomotion")]
        public MovementStateData swimMovementStateData;
        [Header("WallLocomotion")]
        public MovementStateData wallClimbMovementStateData;
        public MovementStateData wallRunMovementStateData;
        [Header("LadderLocomotion")]
        public MovementStateData ladderIdleMovementStateData;
        public MovementStateData ladderClimbMovementStateData;
        public TimedMovementStateData ladderEnterMovementStateData;
        public TimedMovementStateData ladderExitMovementStateData;
        [Header("Actions")]
        public JumpStateData groundJumpMovementStateData;
        public JumpStateData rollMovementStateData;
        public JumpStateData dashMovementStateData;
        public JumpStateData slipMovementStateData;
        public JumpStateData wallJumpMovementStateData;
        public MantlingMovmentStateData mantlingMovementStateData;

        [Header("Stopping")]
        public TimedMovementStateData stoppingMovementStateData;
        [Header("Landing")]
        public TimedMovementStateData landingMovementStateData;
        [Header("Turn")]
        public TurnMovementStateData backTurnMovementStateData;
        public TurnMovementStateData turnMovementStateData;


        private LadderMovementState _ladderMovementState;
        private WallClimbMovementState _wallClimbMovementState;
        private WallRunMovementState _wallRunMovementState;
        private IdleMovementState _idleMovementState;
        private SlipMovementState _slipMovementState;
        private BackTurnMovementState _backTurnMovementState;
        private TurnMovementState _turnMovementState;
        private LandingMovementState _landingMovementState;
        private RunMovementState _runMovementState;
        private WalkMovementState _walkMovementState;
        private FallingMovementState _fallingMovementState;
        private SprintMovementState _sprintMovementState;
        private GroundJumpMovementState _groundJumpMovementState;
        private WallClimbJumpMovementState _wallJumpMovementState;
        private RollMovementState _rollMovementState;
        private DashMovementState _dashMovementState;
        private StopMovementState _stopMovementState;
        private SwimMovementState _swimMovementState;
        private FlyMovementState _flyMovementState;
        private MantlingMovementState _mantlingMovementState;


        private IdleActionState _idleActionState;

        public override void Init(Character character)
        {
            InitializeStates(character);
            ConfigureTransitions(character);
            character.MovementStateMachine.SetState(_idleMovementState);
            character.ActionStateMachine.SetState(_idleActionState);
        }

        // Add AnimationModule to State subscribtion
        private void InitializeStates(Character character)
        {
            _idleMovementState = new IdleMovementState(character, idleMovementStateData);
            _runMovementState = new RunMovementState(character, runMovementStateData);
            _walkMovementState = new WalkMovementState(character, walkMovementStateData);
            _sprintMovementState = new SprintMovementState(character, sprintMovementStateData);
            _fallingMovementState = new FallingMovementState(character, fallingMovementStateData);
            _groundJumpMovementState = new GroundJumpMovementState(character, groundJumpMovementStateData);
            _wallJumpMovementState = new WallClimbJumpMovementState(character, wallJumpMovementStateData);
            _rollMovementState = new RollMovementState(character, rollMovementStateData);
            _backTurnMovementState = new BackTurnMovementState(character, backTurnMovementStateData);
            _landingMovementState = new LandingMovementState(character, landingMovementStateData);
            _dashMovementState = new DashMovementState(character, dashMovementStateData);
            _ladderMovementState = new LadderMovementState(character, ladderClimbMovementStateData);
            _wallClimbMovementState = new WallClimbMovementState(character, wallClimbMovementStateData);
            _stopMovementState = new StopMovementState(character, stoppingMovementStateData);
            _turnMovementState = new TurnMovementState(character, turnMovementStateData);
            _slipMovementState = new SlipMovementState(character, slipMovementStateData);
            _swimMovementState = new SwimMovementState(character, swimMovementStateData);
            _flyMovementState = new FlyMovementState(character, flyMovementStateData);
            _wallRunMovementState = new WallRunMovementState(character, wallRunMovementStateData);
            _mantlingMovementState = new MantlingMovementState(character, mantlingMovementStateData);
            _idleActionState = new IdleActionState(character);
        }

        private bool IsOnLadder()
        {
            return _ladderMovementState.IsActiveState;
        }
        private void ConfigureTransitions(Character character)
        {
            character.AddAnyMovementStateMachineTransition(_rollMovementState, () => character.Input.EvadePressed
            && !_groundJumpMovementState.IsDurationTimerRunning && character.IsGrounded
            && !IsOnLadder());
            character.AddAnyMovementStateMachineTransition(_wallClimbMovementState, () => character.Sensors.IsObstacleLegsFront
            && character.Sensors.LegsFrontHit.collider.CompareTag("Climbable")
            && character.Input.MovementInput.y > 0
            && !IsOnLadder());
            character.AddAnyMovementStateMachineTransition(_fallingMovementState, () => CanFall(character));
            character.AddAnyMovementStateMachineTransition(_swimMovementState, () => character.Sensors.IsInWater);
            //NOTE: SEQUENCE IS IMPORTANT
            // ----- Idle State Transitions -----
            character.AddMovementStateMachineTransition(_idleMovementState, _groundJumpMovementState, () => character.Input.JumpPressed && character.JumpCount < groundJumpMovementStateData.MaxJumpCount && _groundJumpMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_idleMovementState, _turnMovementState, () => _turnMovementState.CanEnter() && Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle);
            character.AddMovementStateMachineTransition(_idleMovementState, _walkMovementState, () => Input.GetKeyDown(KeyCode.Z));
            character.AddMovementStateMachineTransition(_idleMovementState, _runMovementState, () => HasMovementInput(character));
            character.AddMovementStateMachineTransition(_idleMovementState, _ladderMovementState, () => character.GetComponent<LadderMovementController>().Ladder != null && character.Input.InteractPressed);
            //character.AddMovementStateMachineTransition(_idleMovementState, _flyMovementState, () => character.Input.FocusPressed);
            // ----- Walk State Transitions -----
            character.AddMovementStateMachineTransition(_walkMovementState, _idleMovementState, () => Input.GetKeyDown(KeyCode.Z));
            character.AddMovementStateMachineTransition(_walkMovementState, _backTurnMovementState, () => _backTurnMovementState.CanEnter() && Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle);
            // ----- Run State Transitions -----
            character.AddMovementStateMachineTransition(_runMovementState, _dashMovementState, () => _dashMovementState.CanEnter() && character.Input.SprintPressing);
            character.AddMovementStateMachineTransition(_runMovementState, _slipMovementState, () => _slipMovementState.CanEnter() && character.Input.CrouchPressed);
            character.AddMovementStateMachineTransition(_runMovementState, _mantlingMovementState, () => _mantlingMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_fallingMovementState, _mantlingMovementState, () => _mantlingMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_groundJumpMovementState, _mantlingMovementState, () => _mantlingMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_wallJumpMovementState, _mantlingMovementState, () => _mantlingMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_mantlingMovementState, _idleMovementState, () => _mantlingMovementState.IsDurationTimerElapsed);


            character.AddMovementStateMachineTransition(_slipMovementState, _idleMovementState, () => _slipMovementState.IsDurationTimerElapsed && !HasMovementInput(character));
            character.AddMovementStateMachineTransition(_slipMovementState, _runMovementState, () => _slipMovementState.IsDurationTimerElapsed && HasMovementInput(character));
            character.AddMovementStateMachineTransition(_runMovementState, _groundJumpMovementState, () => character.Input.JumpPressed && _groundJumpMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_runMovementState, _backTurnMovementState, () => _backTurnMovementState.CanEnter() && Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle);

            character.AddMovementStateMachineTransition(_runMovementState, _stopMovementState, () => _runMovementState.IsActionFrameElapsed && _stopMovementState.CanEnter() && !HasMovementInput(character));
            character.AddMovementStateMachineTransition(_runMovementState, _turnMovementState, () => _turnMovementState.CanEnter() && Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle && Mathf.Abs(character.DeltaYaw) < turnMovementStateData.MaxAngle && !character.IsAimig);

            // ----- Stop State Transitions -----
            character.AddMovementStateMachineTransition(_stopMovementState, _backTurnMovementState, () => _backTurnMovementState.CanEnter() && Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle);
            character.AddMovementStateMachineTransition(_stopMovementState, _idleMovementState, () => _stopMovementState.IsDurationTimerElapsed);
            character.AddMovementStateMachineTransition(_stopMovementState, _turnMovementState, () => _turnMovementState.CanEnter() && Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle);
            // ----- Sprint State Transitions -----
            character.AddMovementStateMachineTransition(_sprintMovementState, _runMovementState, () => !character.Input.SprintPressing);
            character.AddMovementStateMachineTransition(_sprintMovementState, _stopMovementState, () => _stopMovementState.CanEnter() && (character.Input.MovementInput.magnitude == 0 || Mathf.Abs(character.DeltaYaw) > 120));
            character.AddMovementStateMachineTransition(_sprintMovementState, _groundJumpMovementState, () => character.Input.JumpPressed && character.JumpCount < groundJumpMovementStateData.MaxJumpCount && _groundJumpMovementState.CanEnter());
            // ----- Jump State Transitions -----
            character.AddMovementStateMachineTransition(_groundJumpMovementState, _runMovementState, () => character.Sensors.IsGrounded && _groundJumpMovementState.IsDurationTimerElapsed && HasMovementInput(character));
            character.AddMovementStateMachineTransition(_groundJumpMovementState, _idleMovementState, () => character.Sensors.IsGrounded && _groundJumpMovementState.IsDurationTimerElapsed && !HasMovementInput(character));
            // ----- Dash State Transitions -----
            character.AddMovementStateMachineTransition(_dashMovementState, _sprintMovementState, () => character.Sensors.IsGrounded && character.Input.SprintPressing && !_dashMovementState.IsDurationTimerRunning);
            character.AddMovementStateMachineTransition(_dashMovementState, _stopMovementState, () => character.Sensors.IsGrounded && !HasMovementInput(character) && !_dashMovementState.IsDurationTimerRunning);
            character.AddMovementStateMachineTransition(_dashMovementState, _runMovementState, () => character.Sensors.IsGrounded && HasMovementInput(character) && !_dashMovementState.IsDurationTimerRunning);
            // ----- Falling State Transitions -----
            character.AddMovementStateMachineTransition(_fallingMovementState, _landingMovementState, () => character.Sensors.IsGrounded);
            character.AddMovementStateMachineTransition(_fallingMovementState, _groundJumpMovementState, () => character.Input.JumpPressed && character.JumpCount < groundJumpMovementStateData.MaxJumpCount && _fallingMovementState.IsActionFrameInProgress && _groundJumpMovementState.CanEnter());
            character.AddMovementStateMachineTransition(_groundJumpMovementState, _wallRunMovementState, () => CanWallRun(character));
            // ----- Landing State Transitions -----
            character.AddMovementStateMachineTransition(_landingMovementState, _runMovementState, () => character.Sensors.IsGrounded && _landingMovementState.IsDurationTimerElapsed && HasMovementInput(character));
            character.AddMovementStateMachineTransition(_landingMovementState, _idleMovementState, () => character.Sensors.IsGrounded && _landingMovementState.IsDurationTimerElapsed && !HasMovementInput(character));
            // ----- Roll State Transitions -----
            character.AddMovementStateMachineTransition(_rollMovementState, _runMovementState, () => character.IsGrounded && !_rollMovementState.IsDurationTimerRunning && HasMovementInput(character));
            character.AddMovementStateMachineTransition(_rollMovementState, _idleMovementState, () => character.IsGrounded && !_rollMovementState.IsDurationTimerRunning && !HasMovementInput(character));
            // TurnBack
            character.AddMovementStateMachineTransition(_backTurnMovementState, _runMovementState, () => !_backTurnMovementState.IsDurationTimerRunning && HasMovementInput(character));
            character.AddMovementStateMachineTransition(_backTurnMovementState, _idleMovementState, () => !_backTurnMovementState.IsDurationTimerRunning && !HasMovementInput(character));
            character.AddMovementStateMachineTransition(_turnMovementState, _idleMovementState, () => !_turnMovementState.IsDurationTimerRunning && !HasMovementInput(character));
            character.AddMovementStateMachineTransition(_turnMovementState, _runMovementState, () => !_turnMovementState.IsDurationTimerRunning && HasMovementInput(character));
            // ----- From Ladder State Transitions -----
            character.AddMovementStateMachineTransition(_ladderMovementState, _idleMovementState, () => character.GetComponent<LadderMovementController>().Ladder == null);
            // ----- From Wall Climb State Transitions -----
            character.AddMovementStateMachineTransition(_wallClimbMovementState, _idleMovementState, () => character.Input.InteractPressed || !character.Sensors.IsObstacleLegsFront || !character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") || (character.IsGrounded && character.Input.MovementInput.y < 0));
            character.AddMovementStateMachineTransition(_wallClimbMovementState, _wallJumpMovementState, () => _wallJumpMovementState.CanEnter() && character.Input.JumpPressed);
            character.AddMovementStateMachineTransition(_wallJumpMovementState, _idleMovementState, () => character.IsGrounded);
            character.AddMovementStateMachineTransition(_fallingMovementState, _wallClimbMovementState, () => character.Sensors.IsObstacleLegsFront && character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") && character.Input.MovementInput.y > 0);
            // ----- From Swimming State Transitions -----
            character.AddMovementStateMachineTransition(_swimMovementState, _idleMovementState, () => character.IsGrounded);
            //character.AddMovementStateMachineTransition(_swimMovementState, _runMovementState, () => character.IsGrounded);
            // ----- From Fly State Transitions -----
            character.AddMovementStateMachineTransition(_flyMovementState, _idleMovementState, () => !character.Input.FocusPressed);
            // ----- From Wall Run State Transitions -----
            character.AddMovementStateMachineTransition(_wallRunMovementState, _idleMovementState, () => !CanWallRun(character));

            character.AddAnyActionStateMachineTransition(_idleActionState, () => true);
        }

        private static bool HasMovementInput(Character character)
        {
            return character.Input.MovementInput.magnitude > 0;
        }

        private bool CanWallRun(Character character)
        {
            return     (character.Sensors.IsObstacleLegsLeft || character.Sensors.IsObstacleLegsRight)
                        && !character.Sensors.IsGrounded
                        && character.Input.MovementInput.y > 0
                        && !character.Sensors.IsObstacleLegsFront
                        && Vector3.Angle(character.transform.forward, Camera.main.transform.forward) < 60;
        }
        private bool CanFall(Character character)
        {
            return !character.Sensors.IsGrounded
                        && !_groundJumpMovementState.IsDurationTimerRunning
                        && !_rollMovementState.IsDurationTimerRunning
                        && !IsOnLadder()
                        && !character.Input.JumpPressed
                        && !_wallClimbMovementState.IsActiveState
                        && !_wallJumpMovementState.IsDurationTimerRunning
                        && !character.Sensors.IsInWater
                        && !_flyMovementState.IsActiveState
                        && !_wallRunMovementState.IsActiveState
                        && !_mantlingMovementState.IsActiveState
                        && !_mantlingMovementState.CanEnter(); 
        }
    }
}
