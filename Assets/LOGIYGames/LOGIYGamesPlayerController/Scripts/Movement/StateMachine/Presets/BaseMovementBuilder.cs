using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "BaseMovementPreset", menuName = "MovementStateMachine/MovementStatesPreset/BaseMovementPreset")]
    public partial class BaseMovementBuilder : MovementBuilder
    {

        [SerializeField] List<MovementStateFactory> additionalStartupStates;

        [Header("BASE STATES")]


        [Header("GroundLocomotion")]
        public MovementStateData idleMovementStateData;
        public MovementStateData runMovementStateData;
        public MovementStateData walkMovementStateData;
        public MovementStateData sprintMovementStateData;

        [Header("AirLocomotion")]
        public MovementStateData fallingMovementStateData;

        [Header("WaterLocomotion")]
        public MovementStateData swimMovementStateData;

        [Header("Ladder")]
        public MovementStateData ladderMovementStateData;

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

        public override void Build(Character character)
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
            character.AddMovementState(new FallingMovementState(character, fallingMovementStateData));

            character.AddMovementState(new LandingMovementState(character, landingMovementStateData));

            character.AddMovementState(new IdleMovementState(character, idleMovementStateData));

            character.AddMovementState(new WalkMovementState(character, walkMovementStateData));

            character.AddMovementState(new RunMovementState(character, runMovementStateData));

            character.AddMovementState(new StopMovementState(character, stoppingMovementStateData));

            character.AddMovementState(new TurnMovementState(character, turnMovementStateData));

            character.AddMovementState(new BackTurnMovementState(character, backTurnMovementStateData));

            character.AddMovementState(new SprintMovementState(character, sprintMovementStateData));

            character.AddMovementState(new SwimMovementState(character, swimMovementStateData));

            character.AddMovementState(new LadderMovementState(character, ladderMovementStateData));

            additionalStartupStates.ForEach((e) =>
            {
                e.Create(character);
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

            #region Movement
            // =========================================================
            // IDLE
            // =========================================================
            #region IdleState Transitions

            character.MovementStateMachine.AddTransition
                <IdleMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<GroundJumpMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<TurnMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, WalkMovementState>(
                new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, RunMovementState>(
                new FuncPredicate(() => character.GetMovementState<RunMovementState>().CanEnter()));

            character.MovementStateMachine.AddTransition<IdleMovementState, RollMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<RollMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // WALK
            // =========================================================
            #region WalkState Transitions

            character.MovementStateMachine.AddTransition
                <WalkMovementState, IdleMovementState>(
                new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));

            character.MovementStateMachine.AddTransition
                <WalkMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<BackTurnMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // RUN
            // =========================================================
            #region RunState Transitions

            character.MovementStateMachine.AddTransition
                <RunMovementState, DashMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<DashMovementState>().CanEnter();
                }));


            character.MovementStateMachine.AddTransition
                <RunMovementState, SlipMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<SlipMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<MantlingMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<GroundJumpMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<BackTurnMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<RunMovementState>().IsActionFrameElapsed &&
                        character.GetMovementState<StopMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition
                <RunMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return !character.GetMovementState<StopMovementState>().CanEnter() && character.GetMovementState<IdleMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition
                <RunMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<TurnMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition<RunMovementState, RollMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<RollMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // SLIP
            // =========================================================
            #region SlipState Transitions

            character.MovementStateMachine.AddTransition
                <SlipMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<SlipMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <SlipMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<SlipMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<TurnMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // STOP
            // =========================================================
            #region StopState Transitions

            character.MovementStateMachine.AddTransition
                <StopMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<StopMovementState>().IsDurationTimerElapsed;
                }));

            character.MovementStateMachine.AddTransition
                <StopMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<BackTurnMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <StopMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<TurnMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // SPRINT
            // =========================================================
            #region SprintState Transitions

            character.MovementStateMachine.AddTransition
                <SprintMovementState, RunMovementState>(
                new FuncPredicate(() =>
                    !character.Input.SprintPressing));

            character.MovementStateMachine.AddTransition
                <SprintMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<StopMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <SprintMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<GroundJumpMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // JUMP
            // =========================================================
            #region GroundJumpState Transitions

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<GroundJumpMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<GroundJumpMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, WallRunMovementState>(
                new FuncPredicate(() => character.GetMovementState<WallRunMovementState>().CanEnter()));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<MantlingMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // FALLING
            // =========================================================
            #region FallingState Transitions

            character.MovementStateMachine.AddTransition
                <FallingMovementState, LandingMovementState>(
                new FuncPredicate(() => character.GetMovementState<LandingMovementState>().CanEnter()));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<FallingMovementState>().IsActionFrameInProgress &&
                        character.GetMovementState<GroundJumpMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, WallClimbMovementState>(
                new FuncPredicate(() => character.GetMovementState<WallClimbMovementState>().CanEnter()));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<MantlingMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // LANDING
            // =========================================================
            #region LandingState Transitions

            character.MovementStateMachine.AddTransition
                <LandingMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<LandingMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <LandingMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<LandingMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<IdleMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // DASH
            // =========================================================
            #region DashState Transitions

            character.MovementStateMachine.AddTransition
                <DashMovementState, SprintMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<SprintMovementState>().CanEnter() &&
                        !character.GetMovementState<DashMovementState>().IsDurationTimerRunning;
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<StopMovementState>().CanEnter() &&
                        !character.GetMovementState<DashMovementState>().IsDurationTimerRunning;
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                       character.GetMovementState<RunMovementState>().CanEnter() &&
                        !character.GetMovementState<DashMovementState>().IsDurationTimerRunning;
                }));
            #endregion
            // =========================================================
            // ROLL
            // =========================================================
            #region RollState Transitions

            character.MovementStateMachine.AddTransition
                <RollMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        !character.GetMovementState<RollMovementState>().IsDurationTimerRunning &&
                        character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RollMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                           !character.GetMovementState<RollMovementState>().IsDurationTimerRunning &&
                           character.GetMovementState<IdleMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // TURN
            // =========================================================
            #region TurnState Transitions
            character.MovementStateMachine.AddTransition
                <BackTurnMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return !character.GetMovementState<BackTurnMovementState>().IsDurationTimerRunning &&
                           character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <BackTurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return !character.GetMovementState<BackTurnMovementState>().IsDurationTimerRunning &&
                           character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return !character.GetMovementState<TurnMovementState>().IsDurationTimerRunning &&
                           character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return !character.GetMovementState<TurnMovementState>().IsDurationTimerRunning &&
                          character.GetMovementState<RunMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition
                <TurnMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<GroundJumpMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // WALL CLIMB
            // =========================================================
            #region WallClimb Transitions

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
                    return
                        character.GetMovementState<HangJumpMovementState>().CanEnter() &&
                        character.Input.JumpPressed;
                }));
            #endregion
            // =========================================================
            // WALL JUMP
            // =========================================================
            #region WallJumpState Transitions
            character.MovementStateMachine.AddTransition
                <HangJumpMovementState, IdleMovementState>(
                new FuncPredicate(() => character.IsGrounded));

            character.MovementStateMachine.AddTransition
                <HangJumpMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<MantlingMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // LADDER
            // =========================================================
            #region LadderState Transitions

            character.MovementStateMachine.AddTransition
                <IdleMovementState, LadderMovementState>(
                new FuncPredicate(() => character.GetComponent<LadderClimbController>().Ladder != null && character.Input.InteractPressed));

            character.MovementStateMachine.AddTransition
                <LadderMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetComponent<LadderClimbController>().Ladder == null;
                }));
            #endregion
            // =========================================================
            // MANTLING
            // =========================================================
            #region MantlingState Transitions

            character.MovementStateMachine.AddTransition
                <MantlingMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<MantlingMovementState>().CanExit();
                }));
            #endregion
            // =========================================================
            // SWIM
            // =========================================================
            #region SwimState Transitions

            character.MovementStateMachine.AddTransition
                <SwimMovementState, IdleMovementState>(
                new FuncPredicate(() => character.IsGrounded));
            #endregion
            // =========================================================
            // FLY
            // =========================================================
            #region FlyState Transitions

            character.MovementStateMachine.AddTransition
                <FlyMovementState, IdleMovementState>(
                new FuncPredicate(() => !character.Input.FocusPressed));
            #endregion
            // =========================================================
            // WALL RUN
            // =========================================================
            #region WallRunState Transitions

            character.MovementStateMachine.AddTransition
                <WallRunMovementState, IdleMovementState>(
                new FuncPredicate(() => !character.GetMovementState<WallRunMovementState>().CanEnter()));
            #endregion
            #endregion
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private bool CanFall(Character character)
        {
            var groundJump = character.GetMovementState<GroundJumpMovementState>();
            var roll = character.GetMovementState<RollMovementState>();
            var hangJump = character.GetMovementState<HangJumpMovementState>();
            var ladder = character.GetMovementState<LadderMovementState>();
            var wallClimb = character.GetMovementState<WallClimbMovementState>();
            var swim = character.GetMovementState<SwimMovementState>();
            var fly = character.GetMovementState<FlyMovementState>();
            var wallRun = character.GetMovementState<WallRunMovementState>();
            var mantling = character.GetMovementState<MantlingMovementState>();

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
    }
}