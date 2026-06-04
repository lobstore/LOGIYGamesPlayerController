using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "BaseMovementPreset", menuName = "MovementStateMachine/MovementStatesPreset/BaseMovementPreset")]
    public partial class BaseMovementPreset : MovementBuilder
    {

        [SerializeField] List<MovementStateFactory> additionalStartupStates;

        [Header("BASE STATES")]


        [Header("GroundLocomotion")]
        public MovementStateData idleMovementStateData;
        public MovementStateData runMovementStateData;
        public MovementStateData walkMovementStateData;
        public MovementStateData sprintMovementStateData;
        public MovementStateData comboStateData;
        public MovementStateData abilityStateData;

        [Header("AirLocomotion")]
        public MovementStateData fallingMovementStateData;

        [Header("WaterLocomotion")]
        public MovementStateData swimMovementStateData;

        [Header("Ladder")]
        public MovementStateData ladderMovementStatData;

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

        public override void Build(CharacterModule character)
        {
            RegisterStates(character);

            ConfigureTransitions(character);

            character.MovementStateMachine.SetState<IdleMovementState>();
        }

        // =========================================================
        // STATE REGISTRATION
        // =========================================================

        private void RegisterStates(CharacterModule character)
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

            character.AddMovementState(new LadderMovementState(character, ladderMovementStatData));
            character.AddMovementState(new AbilityMovementState(character, abilityStateData));

            character.AddMovementState(
    new ComboMovementState(
        character,
        comboStateData));
            additionalStartupStates.ForEach((e) =>
            {
                e.Create(character);
            });
        }

        private void ConfigureTransitions(CharacterModule character)
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
            character.MovementStateMachine.AddAnyTransition<AbilityMovementState>(
            new FuncPredicate(() => character.AbilityController.CurrentAbility != null && character.AbilityController.Phase != AbilityPhase.Finished));

            #region Movement
            // =========================================================
            // IDLE
            // =========================================================
            #region IdleState Transitions

            character.MovementStateMachine.AddTransition
                <IdleMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetMovementState<GroundJumpMovementState>();

                    return jump.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <IdleMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetMovementState<TurnMovementState>();

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
                    var roll = character.GetMovementState<RollMovementState>();

                    return roll.CanEnter();
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
                    var backTurn = character.GetMovementState<BackTurnMovementState>();

                    return
                        backTurn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle;
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
                    var dash = character.GetMovementState<DashMovementState>();

                    return
                        dash.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, SlipMovementState>(
                new FuncPredicate(() =>
                {
                    var slip = character.GetMovementState<SlipMovementState>();

                    return
                        slip.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, MantlingMovementState>(
                new FuncPredicate(() =>
                {
                    var mantling = character.GetMovementState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetMovementState<GroundJumpMovementState>();

                    return
                        jump.CanEnter();
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetMovementState<BackTurnMovementState>();

                    return
                        backTurn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle;
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    var run = character.GetMovementState<RunMovementState>();
                    var stop = character.GetMovementState<StopMovementState>();

                    return
                        run.IsActionFrameElapsed &&
                        stop.CanEnter() &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <RunMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetMovementState<TurnMovementState>();

                    return
                        turn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle &&
                        Mathf.Abs(character.DeltaYaw) < turnMovementStateData.MaxAngle &&
                        !character.IsAimig;
                }));
            character.MovementStateMachine.AddTransition<RunMovementState, RollMovementState>(
                new FuncPredicate(() =>
                {
                    var roll = character.GetMovementState<RollMovementState>();

                    return roll.CanEnter();
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
                    var slip = character.GetMovementState<SlipMovementState>();

                    return
                        slip.IsDurationTimerElapsed &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <SlipMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var slip = character.GetMovementState<SlipMovementState>();

                    return
                        slip.IsDurationTimerElapsed &&
                        HasMovementInput(character);
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
                    var stop = character.GetMovementState<StopMovementState>();

                    return stop.IsDurationTimerElapsed;
                }));

            character.MovementStateMachine.AddTransition
                <StopMovementState, BackTurnMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetMovementState<BackTurnMovementState>();

                    return
                        backTurn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > backTurnMovementStateData.MinAngle;
                }));

            character.MovementStateMachine.AddTransition
                <StopMovementState, TurnMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetMovementState<TurnMovementState>();

                    return
                        turn.CanEnter() &&
                        Mathf.Abs(character.DeltaYaw) > turnMovementStateData.MinAngle;
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
                    var stop = character.GetMovementState<StopMovementState>();

                    return
                        stop.CanEnter() &&
                        (character.Input.MovementInput.magnitude == 0 ||
                         Mathf.Abs(character.DeltaYaw) > 120);
                }));

            character.MovementStateMachine.AddTransition
                <SprintMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetMovementState<GroundJumpMovementState>();

                    return jump.CanEnter();
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
                    var jump = character.GetMovementState<GroundJumpMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        jump.IsDurationTimerElapsed &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <GroundJumpMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetMovementState<GroundJumpMovementState>();

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
                    var mantling = character.GetMovementState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));
            #endregion
            // =========================================================
            // FALLING
            // =========================================================
            #region FallingState Transitions

            character.MovementStateMachine.AddTransition
                <FallingMovementState, LandingMovementState>(
                new FuncPredicate(() => character.Sensors.IsGrounded));

            character.MovementStateMachine.AddTransition
                <FallingMovementState, GroundJumpMovementState>(
                new FuncPredicate(() =>
                {
                    var jump = character.GetMovementState<GroundJumpMovementState>();
                    var falling = character.GetMovementState<FallingMovementState>();

                    return
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
                    var mantling = character.GetMovementState<MantlingMovementState>();

                    return mantling.CanEnter();
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
                    var landing = character.GetMovementState<LandingMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        landing.IsDurationTimerElapsed &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <LandingMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var landing = character.GetMovementState<LandingMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        landing.IsDurationTimerElapsed &&
                        !HasMovementInput(character);
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
                    var dash = character.GetMovementState<DashMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        character.Input.SprintPressing &&
                        !dash.IsDurationTimerRunning;
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, StopMovementState>(
                new FuncPredicate(() =>
                {
                    var dash = character.GetMovementState<DashMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        !HasMovementInput(character) &&
                        !dash.IsDurationTimerRunning;
                }));

            character.MovementStateMachine.AddTransition
                <DashMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var dash = character.GetMovementState<DashMovementState>();

                    return
                        character.Sensors.IsGrounded &&
                        HasMovementInput(character) &&
                        !dash.IsDurationTimerRunning &&
                        !character.Input.SprintPressing;
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
                    var roll = character.GetMovementState<RollMovementState>();

                    return
                        character.IsGrounded &&
                        !roll.IsDurationTimerRunning &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <RollMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var roll = character.GetMovementState<RollMovementState>();

                    return
                        character.IsGrounded &&
                        !roll.IsDurationTimerRunning &&
                        !HasMovementInput(character);
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
                    var backTurn = character.GetMovementState<BackTurnMovementState>();

                    return
                        !backTurn.IsDurationTimerRunning &&
                        HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <BackTurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var backTurn = character.GetMovementState<BackTurnMovementState>();

                    return
                        !backTurn.IsDurationTimerRunning &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetMovementState<TurnMovementState>();

                    return
                        !turn.IsDurationTimerRunning &&
                        !HasMovementInput(character);
                }));

            character.MovementStateMachine.AddTransition
                <TurnMovementState, RunMovementState>(
                new FuncPredicate(() =>
                {
                    var turn = character.GetMovementState<TurnMovementState>();

                    return
                        !turn.IsDurationTimerRunning &&
                        HasMovementInput(character);
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
                    var wallJump = character.GetMovementState<HangJumpMovementState>();

                    return
                        wallJump.CanEnter() &&
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
                    var mantling = character.GetMovementState<MantlingMovementState>();

                    return mantling.CanEnter();
                }));
            #endregion
            // =========================================================
            // LADDER
            // =========================================================
            #region LadderState Transitions

            character.MovementStateMachine.AddTransition
                <IdleMovementState, LadderMovementState>(
                new FuncPredicate(() => character.GetComponent<LadderMovementController>().Ladder != null && character.Input.InteractPressed));

            character.MovementStateMachine.AddTransition
                <LadderMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.GetComponent<LadderMovementController>().Ladder == null;
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
                    var mantling = character.GetMovementState<MantlingMovementState>();

                    return mantling.IsDurationTimerElapsed;
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
                new FuncPredicate(() => !CanWallRun(character)));
            #endregion
            #endregion
            #region Combo
            /*
            ========================================================
            IDLE -> COMBO
            ========================================================
            */


            character.MovementStateMachine.AddTransition<IdleMovementState, ComboMovementState>(
                new FuncPredicate(() =>
                    character.Input.AttackPressed && character.WeaponController.CurrentWeapon != null));


            /*
            ========================================================
            RUN -> COMBO
            ========================================================
            */


            character.MovementStateMachine.AddTransition<RunMovementState, ComboMovementState>(
                    new FuncPredicate(() => character.Input.AttackPressed)
                );


            /*
            ========================================================
            COMBO -> IDLE
            ========================================================
            */


            character.MovementStateMachine.AddTransition<ComboMovementState, IdleMovementState>(
                new FuncPredicate(() =>
                {
                    return character.ComboController.IsFinished() && !HasMovementInput(character);
                }));


            /*
            ========================================================
            COMBO -> RUN
            ========================================================
            */

            character.MovementStateMachine.AddTransition
                <ComboMovementState,
                 RunMovementState>(
                new FuncPredicate(() =>
                {
                    return character.ComboController.IsFinished() && HasMovementInput(character);
                }));


            /*
            ========================================================
            COMBO -> ROLL
            ========================================================
            */

            character.MovementStateMachine.AddTransition
                <ComboMovementState,
                 RollMovementState>(
                new FuncPredicate(() =>
                {
                    var combo =
                        character.GetMovementState
                            <ComboMovementState>();

                    var roll =
                        character.GetMovementState
                            <RollMovementState>();

                    return combo.CanExit() && roll.CanEnter();
                }));
            #endregion
            #region Ability
            character.MovementStateMachine.AddTransition
    <AbilityMovementState, IdleMovementState>(
            new FuncPredicate(() =>
            {
                var ability = character.GetMovementState<AbilityMovementState>();
                return ability.CanExit();
            }
        ));

            #endregion
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private static bool HasMovementInput(CharacterModule character)
        {
            return character.Input.MovementInput.magnitude > 0;
        }

        private bool CanFall(CharacterModule character)
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
            var skill = character.GetMovementState<AbilityMovementState>();

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
            && (groundJump == null || !groundJump.CanEnter())
            && (skill == null || !skill.IsActiveState);
        }
        private bool CanWallRun(CharacterModule character)
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