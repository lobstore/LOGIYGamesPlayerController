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

        [Header("AirLocomotion")]
        public MovementStateData fallingMovementStateData;


        // =========================================================
        // INIT
        // =========================================================

        public override void Build(Actor character)
        {
            RegisterStates(character);

            ConfigureTransitions(character);

            character.MovementStateMachine.SetState<IdleMovementState>();
        }

        // =========================================================
        // STATE REGISTRATION
        // =========================================================

        private void RegisterStates(Actor character)
        {
            character.AddMovementState(new FallingMovementState(character, fallingMovementStateData));

            character.AddMovementState(new IdleMovementState(character, idleMovementStateData));

            character.AddMovementState(new RunMovementState(character, runMovementStateData));

            additionalStartupStates.ForEach((state) =>
            {
                state.Create(character);
            });
        }

        private void ConfigureTransitions(Actor character)
        {
            // =========================================================
            // ANY TRANSITIONS
            // =========================================================
            #region Any Transitions
            character.MovementStateMachine.AddAnyTransition<WallClimbMovementState>(
                new FuncPredicate(() => character.GetMovementState<WallClimbMovementState>().CanEnter() && !character.GetMovementState<MantlingMovementState>().IsActiveState));

            character.MovementStateMachine.AddAnyTransition<FallingMovementState>(
                new FuncPredicate(() => CanFall(character)));

            character.MovementStateMachine.AddAnyTransition<SwimMovementState>(
                new FuncPredicate(() => character.GetMovementState<SwimMovementState>().CanEnter()));
            character.MovementStateMachine.AddAnyTransition<MountingMovementState>(
                new FuncPredicate(() => character.GetMovementState<MountingMovementState>().CanEnter()));
            character.MovementStateMachine.AddAnyTransition<FlyMovementState>(
                new FuncPredicate(() => character.GetMovementState<FlyMovementState>().CanEnter()));

            #endregion
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
                <IdleMovementState, CrouchMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<CrouchMovementState>().CanEnter();
                }));

            //character.MovementStateMachine.AddTransition
            //    <IdleMovementState, WalkMovementState>(
            //    new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, ComboMovementState>(
                new FuncPredicate(() => character.GetMovementState<ComboMovementState>().CanEnter()
                ));
            character.MovementStateMachine.AddTransition
    <ComboMovementState, IdleMovementState>(
    new FuncPredicate(() => character.GetMovementState<ComboMovementState>().CanExit()
    ));
            character.MovementStateMachine.AddTransition
                <IdleMovementState, BackTurnMovementState>(
                new FuncPredicate(() => character.GetMovementState<BackTurnMovementState>().CanEnter()
                ));
            character.MovementStateMachine.AddTransition
                <IdleMovementState, RunMovementState>(
                new FuncPredicate(() => character.GetMovementState<RunMovementState>().CanEnter()
                ));

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
                <RunMovementState, SlipJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<SlipJumpMovementState>().CanEnter();
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
                    return character.GetMovementState<StopMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition
                <RunMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                     character.GetMovementState<IdleMovementState>().CanEnter();
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
                <SlipJumpMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<SlipJumpMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <SlipJumpMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<SlipJumpMovementState>().IsDurationTimerElapsed &&
                        character.GetMovementState<RunMovementState>().CanEnter();
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
                    return character.GetMovementState<StopMovementState>().CanExit();
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
                    character.GetMovementState<SprintMovementState>().CanExit()
                    && character.GetMovementState<RunMovementState>().CanEnter()));

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
            character.MovementStateMachine.AddTransition
                <SprintMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<SprintMovementState>().CanExit()
                    && character.GetMovementState<IdleMovementState>().CanEnter();
                }));
            #endregion
            // =========================================================
            // JUMP
            // =========================================================
            #region GroundJumpState Transitions


            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, LandingMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<GroundJumpMovementState>().CanExit() &&
                        character.GetMovementState<LandingMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<GroundJumpMovementState>().CanExit() &&
                        character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<GroundJumpMovementState>().CanExit() &&
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
                <FallingMovementState, IdleMovementState>(
                new FuncPredicate(() => character.GetMovementState<IdleMovementState>().CanEnter()));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, RunMovementState>(
                new FuncPredicate(() => character.GetMovementState<RunMovementState>().CanEnter()));

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
                        character.GetMovementState<LandingMovementState>().CanExit() &&
                        character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <LandingMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<LandingMovementState>().CanExit() &&
                        character.GetMovementState<IdleMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition
                <LandingMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<LandingMovementState>().IsActionFrameInProgress &&
                        character.GetMovementState<GroundJumpMovementState>().CanEnter();
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
                        character.GetMovementState<DashMovementState>().CanExit() &&
                        character.GetMovementState<SprintMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<DashMovementState>().CanExit() &&
                    character.GetMovementState<StopMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<DashMovementState>().CanExit() &&
                       character.GetMovementState<RunMovementState>().CanEnter();
                }));
            character.MovementStateMachine.AddTransition
                <DashMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<DashMovementState>().CanExit() &&
                       character.GetMovementState<IdleMovementState>().CanEnter();
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
                        character.GetMovementState<RollMovementState>().CanExit() &&
                        character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RollMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return
                           character.GetMovementState<RollMovementState>().CanExit() &&
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
                    return character.GetMovementState<BackTurnMovementState>().CanExit() &&
                           character.GetMovementState<RunMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <BackTurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<BackTurnMovementState>().CanExit() &&
                           character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<TurnMovementState>().CanExit() &&
                           character.GetMovementState<IdleMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<TurnMovementState>().CanExit() &&
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
                character.GetMovementState<WallClimbMovementState>().CanExit()));

            character.MovementStateMachine.AddTransition
                <WallClimbMovementState, HangJumpMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<HangJumpMovementState>().CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <WallClimbMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    return
                        character.GetMovementState<MantlingMovementState>().CanEnter();
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
                new FuncPredicate(() => character.GetMovementState<LadderMovementState>().CanEnter()));

            character.MovementStateMachine.AddTransition
                <LadderMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<LadderMovementState>().CanExit();
                }));
            #endregion
            // =========================================================
            // MANTLING
            // =========================================================
            #region MantlingState Transitions

            character.MovementStateMachine.AddTransition<MantlingMovementState, IdleMovementState>(
                new FuncPredicate(
                    () => character.GetMovementState<MantlingMovementState>().CanExit())
                );
            #endregion
            // =========================================================
            // SWIM
            // =========================================================
            #region SwimState Transitions

            character.MovementStateMachine.AddTransition<SwimMovementState, IdleMovementState>(
                new FuncPredicate(
                    () => character.GetMovementState<SwimMovementState>().CanExit()
                          && character.GetMovementState<IdleMovementState>().CanEnter())
                );
            #endregion
            // =========================================================
            // FLY
            // =========================================================
            #region FlyState Transitions

            character.MovementStateMachine.AddTransition
                <FlyMovementState, IdleMovementState>(
                new FuncPredicate(() => character.GetMovementState<FlyMovementState>().CanExit()
                ));
            #endregion
            // =========================================================
            // WALL RUN
            // =========================================================
            #region WallRunState Transitions

            character.MovementStateMachine.AddTransition
                <WallRunMovementState, IdleMovementState>(
                new FuncPredicate(() => !character.GetMovementState<WallRunMovementState>().CanEnter()));
            #endregion

            // =========================================================
            // Crouch
            // =========================================================
            #region CrouchState Transitions

            character.MovementStateMachine.AddTransition
                <CrouchMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetMovementState<CrouchMovementState>().CanExit();
                }));
            #endregion

            // =========================================================
            // Mounting
            // =========================================================
            #region MountingState Transitions

            character.MovementStateMachine.AddTransition
                <MountingMovementState, IdleMovementState>(
                new FuncPredicate(() => character.GetMovementState<MountingMovementState>().CanExit()
                ));
            #endregion
            #endregion
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private bool CanFall(Actor character)
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
            var mounting = character.GetMovementState<MountingMovementState>();

            return !character.IsGrounded
                && (groundJump == null || !groundJump.IsDurationTimerRunning)
            && (roll == null || !roll.IsDurationTimerRunning)
            && (hangJump == null || !hangJump.IsDurationTimerRunning)
            && (ladder == null || !ladder.IsActiveState)
            && (wallClimb == null || !wallClimb.IsActiveState)
            && (swim == null || !swim.IsActiveState)
            && (fly == null || !fly.IsActiveState)
            && (wallRun == null || !wallRun.IsActiveState)
            && (mantling == null || !mantling.IsActiveState)
            && (mounting == null || !mounting.IsActiveState)
            && (mantling == null || !mantling.CanEnter())
            && (groundJump == null || !groundJump.CanEnter());
        }
    }
}