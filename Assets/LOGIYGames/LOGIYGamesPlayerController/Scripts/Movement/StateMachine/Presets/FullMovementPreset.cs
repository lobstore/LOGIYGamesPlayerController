using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace LOGIYGames
{
    [CreateAssetMenu(
        fileName = "FullMovementPreset",
        menuName = "MovementStateMachine/MovementStatesPreset/FullMovementPreset")]
    public class FullMovementPreset : MovementStatesPresetBase
    {

        [SerializeField] List<MovementStateSO> m_states; 

        [Header("GroundLocomotion")]
        public MovementStateData idleMovementStateData;
        public MovementStateData runMovementStateData;
        public MovementStateData walkMovementStateData;
        public MovementStateData sprintMovementStateData;
        public MovementStateData slidingMovementStateData;

        [Header("AirLocomotion")]
        public MovementStateData flyMovementStateData;
        public MovementStateData fallingMovementStateData;

        [Header("WaterLocomotion")]
        public MovementStateData swimMovementStateData;

        [Header("Ladder")]
        public MovementStateData ladderMovementStatData;

        [Header("WallLocomotion")]
        public MovementStateData wallClimbMovementStateData;
        public MovementStateData wallRunMovementStateData;

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


        // =========================================================
        // INIT
        // =========================================================

        public override void Init(Character character)
        {
            RegisterStates(character);

            ConfigureTransitions(character);

            character.MovementStateMachine.SetState<IdleMovementState>();
        }

        // =========================================================
        // STATE REGISTRATION
        // =========================================================

        private void RegisterStates(Character character)
        {
            character.AddState(new FallingMovementState(character, fallingMovementStateData));

            character.AddState(new LandingMovementState(character, landingMovementStateData));

            character.AddState(new IdleMovementState(character, idleMovementStateData));

            character.AddState(new WalkMovementState(character, walkMovementStateData));

            character.AddState(new RunMovementState(character, runMovementStateData));

            character.AddState(new StopMovementState(character, stoppingMovementStateData));

            character.AddState(new TurnMovementState(character, turnMovementStateData));

            character.AddState(new BackTurnMovementState(character, backTurnMovementStateData));

            //character.AddState(new SprintMovementState(character, sprintMovementStateData));

            //character.AddState(new SlipMovementState(character, slipMovementStateData));

            //character.AddState(new SwimMovementState(character, swimMovementStateData));

            //character.AddState(new FlyMovementState(character, flyMovementStateData));

            //character.AddState(new WallRunMovementState(character, wallRunMovementStateData));

            //character.AddState(new WallClimbMovementState(character, wallClimbMovementStateData));

            //character.AddState(new HangJumpMovementState(character, wallJumpMovementStateData));

            //character.AddState(new LadderMovementState(character, ladderMovementStatData));

            //character.AddState(new GroundJumpMovementState(character, groundJumpMovementStateData));

            //character.AddState(new RollMovementState(character, rollMovementStateData));

            //character.AddState(new DashMovementState(character, dashMovementStateData));

            //character.AddState(new MantlingMovementState(character, mantlingMovementStateData));
            m_states.ForEach((e) =>
            {
               e.Build(character);
            });
        }

        private void ConfigureTransitions(Character character)
        {
            // =========================================================
            // ANY TRANSITIONS
            // =========================================================



            character.MovementStateMachine.AddAnyTransition<WallClimbMovementState>(
                new FuncPredicate(() =>
                    character.Sensors.IsObstacleLegsFront &&
                    character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") &&
                    character.Input.MovementInput.y > 0));

            character.MovementStateMachine.AddAnyTransition<FallingMovementState>(
                new FuncPredicate(() => CanFall(character)));

            character.MovementStateMachine.AddAnyTransition<SwimMovementState>(
                new FuncPredicate(() => character.Sensors.IsInWater));

            // =========================================================
            // IDLE
            // =========================================================

            character.MovementStateMachine.AddTransition
                <IdleMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetState<GroundJumpMovementState>();

                    return jump.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetState<TurnMovementState>();

                    return
                        turn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle;
                }));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, WalkMovementState>(
                new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, RunMovementState>(
                new FuncPredicate(() => HasMovementInput(character)));

            character.MovementStateMachine.AddTransition<IdleMovementState, RollMovementState>(
                new FuncPredicate(() =>
                {
                    var roll = character.GetState<RollMovementState>();

                    return roll.CanEnter();
                }));

            // =========================================================
            // WALK
            // =========================================================

            character.MovementStateMachine.AddTransition
                <WalkMovementState, IdleMovementState>(
                new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));

            character.MovementStateMachine.AddTransition
                <WalkMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetState<BackTurnMovementState>();

                    return
                        backTurn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle;
                }));

            // =========================================================
            // RUN
            // =========================================================

            character.MovementStateMachine.AddTransition
                <RunMovementState, DashMovementState>(
                new FuncPredicate(() =>
                {
                    var dash = character.GetState<DashMovementState>();

                    return
                        dash.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, SlipMovementState>(
                new FuncPredicate(() =>
                {
                    var slip = character.GetState<SlipMovementState>();

                    return
                        slip.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    var mantling = character.GetState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetState<GroundJumpMovementState>();

                    return
                        jump.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetState<BackTurnMovementState>();

                    return
                        backTurn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle;
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    var run = character.GetState<RunMovementState>();
                    var stop = character.GetState<StopMovementState>();

                    return
                        run.IsActionFrameElapsed &&
                        stop.CanEnter() &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetState<TurnMovementState>();

                    return
                        turn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle &&
                        Mathf.Abs(character.DeltaYaw) < turnMovementStateData.MaxAngle &&
                        !character.IsAimig;
                }));
            character.MovementStateMachine.AddTransition<RunMovementState, RollMovementState>(
                new FuncPredicate(() =>
                {
                    var roll = character.GetState<RollMovementState>();

                    return roll.CanEnter();
                }));
            // =========================================================
            // SLIP
            // =========================================================

            character.MovementStateMachine.AddTransition
                <SlipMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var slip = character.GetState<SlipMovementState>();

                    return
                        slip.IsDurationTimerElapsed &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <SlipMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var slip = character.GetState<SlipMovementState>();

                    return
                        slip.IsDurationTimerElapsed &&
                        HasMovementInput(character);
                }));

            // =========================================================
            // STOP
            // =========================================================

            character.MovementStateMachine.AddTransition
                <StopMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var stop = character.GetState<StopMovementState>();

                    return stop.IsDurationTimerElapsed;
                }));

            character.MovementStateMachine.AddTransition
                <StopMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetState<BackTurnMovementState>();

                    return
                        backTurn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle;
                }));

            character.MovementStateMachine.AddTransition
                <StopMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetState<TurnMovementState>();

                    return
                        turn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle;
                }));

            // =========================================================
            // SPRINT
            // =========================================================

            character.MovementStateMachine.AddTransition
                <SprintMovementState, RunMovementState>(
                new FuncPredicate(() =>
                    !character.Input.SprintPressing));

            character.MovementStateMachine.AddTransition
                <SprintMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    var stop = character.GetState<StopMovementState>();

                    return
                        stop.CanEnter() &&
                        (character.Input.MovementInput.magnitude == 0 ||
                         Mathf.Abs(character.DeltaYaw) > 120);
                }));

            character.MovementStateMachine.AddTransition
                <SprintMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetState<GroundJumpMovementState>();

                    return jump.CanEnter();
                }));

            // =========================================================
            // JUMP
            // =========================================================

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetState<GroundJumpMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        jump.IsDurationTimerElapsed &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetState<GroundJumpMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        jump.IsDurationTimerElapsed &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, WallRunMovementState>(
                new FuncPredicate(() => CanWallRun(character)));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    var mantling = character.GetState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));

            // =========================================================
            // FALLING
            // =========================================================

            character.MovementStateMachine.AddTransition
                <FallingMovementState, LandingMovementState>(
                new FuncPredicate(() => character.Sensors.IsGrounded));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetState<GroundJumpMovementState>();
                    var falling = character.GetState<FallingMovementState>();

                    return
                        character.JumpCount < groundJumpMovementStateData.MaxJumpCount &&
                        falling.IsActionFrameInProgress &&
                        jump.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, WallClimbMovementState>(
                new FuncPredicate(() =>
                    character.Sensors.IsObstacleLegsFront &&
                    character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") &&
                    character.Input.MovementInput.y > 0));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    var mantling = character.GetState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));

            // =========================================================
            // LANDING
            // =========================================================

            character.MovementStateMachine.AddTransition
                <LandingMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var landing = character.GetState<LandingMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        landing.IsDurationTimerElapsed &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <LandingMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var landing = character.GetState<LandingMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        landing.IsDurationTimerElapsed &&
                        !HasMovementInput(character);
                }));

            // =========================================================
            // DASH
            // =========================================================

            character.MovementStateMachine.AddTransition
                <DashMovementState, SprintMovementState>(
                new FuncPredicate(() =>
                {
                    var dash = character.GetState<DashMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        character.Input.SprintPressing &&
                        !dash.IsDurationTimerRunning;
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    var dash = character.GetState<DashMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        !HasMovementInput(character) &&
                        !dash.IsDurationTimerRunning;
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var dash = character.GetState<DashMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        HasMovementInput(character) &&
                        !dash.IsDurationTimerRunning;
                }));

            // =========================================================
            // ROLL
            // =========================================================

            character.MovementStateMachine.AddTransition
                <RollMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var roll = character.GetState<RollMovementState>();

                    return
                        character.IsGrounded &&
                        !roll.IsDurationTimerRunning &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <RollMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var roll = character.GetState<RollMovementState>();

                    return
                        character.IsGrounded &&
                        !roll.IsDurationTimerRunning &&
                        !HasMovementInput(character);
                }));

            // =========================================================
            // TURN
            // =========================================================

            character.MovementStateMachine.AddTransition
                <BackTurnMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetState<BackTurnMovementState>();

                    return
                        !backTurn.IsDurationTimerRunning &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <BackTurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetState<BackTurnMovementState>();

                    return
                        !backTurn.IsDurationTimerRunning &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetState<TurnMovementState>();

                    return
                        !turn.IsDurationTimerRunning &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetState<TurnMovementState>();

                    return
                        !turn.IsDurationTimerRunning &&
                        HasMovementInput(character);
                }));

            // =========================================================
            // WALL CLIMB
            // =========================================================

            character.MovementStateMachine.AddTransition
                <WallClimbMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                    character.Input.InteractPressed ||
                    !character.Sensors.IsObstacleLegsFront ||
                    !character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") ||
                    (character.IsGrounded && character.Input.MovementInput.y < 0)));

            character.MovementStateMachine.AddTransition
                <WallClimbMovementState, HangJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var wallJump = character.GetState<HangJumpMovementState>();

                    return
                        wallJump.CanEnter() &&
                        character.Input.JumpPressed;
                }));

            // =========================================================
            // WALL JUMP
            // =========================================================

            character.MovementStateMachine.AddTransition
                <HangJumpMovementState, IdleMovementState>(
                new FuncPredicate(() => character.IsGrounded));

            character.MovementStateMachine.AddTransition
                <HangJumpMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    var mantling = character.GetState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));

            // =========================================================
            // LADDER
            // =========================================================

            character.MovementStateMachine.AddTransition
                <IdleMovementState, LadderMovementState>(
                new FuncPredicate(() => character.GetComponent<LadderMovementController>().Ladder != null && character.Input.InteractPressed));

            character.MovementStateMachine.AddTransition
                <LadderMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetComponent<LadderMovementController>().Ladder == null;
                }));

            // =========================================================
            // MANTLING
            // =========================================================

            character.MovementStateMachine.AddTransition
                <MantlingMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var mantling = character.GetState<MantlingMovementState>();

                    return mantling.IsDurationTimerElapsed;
                }));

            // =========================================================
            // SWIM
            // =========================================================

            character.MovementStateMachine.AddTransition
                <SwimMovementState, IdleMovementState>(
                new FuncPredicate(() => character.IsGrounded));

            // =========================================================
            // FLY
            // =========================================================

            character.MovementStateMachine.AddTransition
                <FlyMovementState, IdleMovementState>(
                new FuncPredicate(() => !character.Input.FocusPressed));

            // =========================================================
            // WALL RUN
            // =========================================================

            character.MovementStateMachine.AddTransition
                <WallRunMovementState, IdleMovementState>(
                new FuncPredicate(() => !CanWallRun(character)));
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private static bool HasMovementInput(Character character)
        {
            return character.Input.MovementInput.magnitude > 0;
        }

        private bool CanFall(Character character)
        {
            var groundJump = character.GetState<GroundJumpMovementState>();
            var roll = character.GetState<RollMovementState>();
            var hangJump = character.GetState<HangJumpMovementState>();
            var ladder = character.GetState<LadderMovementState>();
            var wallClimb = character.GetState<WallClimbMovementState>();
            var swim = character.GetState<SwimMovementState>();
            var fly = character.GetState<FlyMovementState>();
            var wallRun = character.GetState<WallRunMovementState>();
            var mantling = character.GetState<MantlingMovementState>();

            return !character.IsGrounded
                && (groundJump == null || !groundJump.IsDurationTimerRunning)
            && (roll == null || !roll.IsActiveState)
            && (hangJump == null || !hangJump.IsActiveState)
            && (ladder == null || !ladder.IsActiveState)
            && (wallClimb == null || !wallClimb.IsActiveState)
            && (swim == null || !swim.IsActiveState)
            && (fly == null || !fly.IsActiveState)
            && (wallRun == null || !wallRun.IsActiveState)
            && (mantling == null || !mantling.IsActiveState)
            && (mantling == null || !mantling.CanEnter())
            && (groundJump == null || !groundJump.CanEnter());
        }
        private bool CanWallRun(Character character)
        {
            return (character.Sensors.IsObstacleLegsLeft
                || character.Sensors.IsObstacleLegsRight)
                && !character.Sensors.IsGrounded
                && character.Input.MovementInput.y > 0
                && !character.Sensors.IsObstacleLegsFront
                && Vector3.Angle(character.transform.forward, Camera.main.transform.forward) < 60;
        }
    }
}