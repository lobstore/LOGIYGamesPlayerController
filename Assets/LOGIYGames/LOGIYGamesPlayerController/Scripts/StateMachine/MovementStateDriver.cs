using LOGIYGames.CharacterCore;
using System.Net.NetworkInformation;
using UnityEngine;

namespace LOGIYGames
{
    public class MovementStateDriver : MonoBehaviour
    {
        StateMachine StateMachine;
        public Character Character;
        public SensorsModule Sensors;



        string currentState;
        string lastTransition;
        [SerializeField] private StatesDataSO statesDataSO;

        IdleState idleState;
        WalkState walkState;
        RunState runState;
        SprintState sprintState;

        FallingState fallingState;
        LandingState landingState;
        JumpState groundJumpState;
        StopState stopState;
        CrouchState crouchState;
        RollState rollState;

        //WallrunState wallrunState;
        //ClimbWallState climbState;
        //WallJumpState wallJumpState;

        //SlideState slideState;
        //SlipState slipState;

        //DashState dashState;
        //FlyState flyState;
        //SwimState swimState;
        //LedgeHangingState ledgeHangingState;
        //LedgeHangUpState hangUpState;
        //JumpDown jumpDownState;
        void Start()
        {
            StateMachine = new();

            idleState = new IdleState(this, statesDataSO.IdleStateData);
            walkState = new WalkState(this, statesDataSO.WalkStateData);
            runState = new RunState(this, statesDataSO.RunStateData);
            sprintState = new SprintState(this, statesDataSO.SprintStateData);
            fallingState = new FallingState(this, statesDataSO.FallingStateData);
            landingState = new LandingState(this, statesDataSO.LandingStateData);
            groundJumpState = new JumpState(this, statesDataSO.GroundJumpStateData);
            rollState = new RollState(this, statesDataSO.RollStateData);
            stopState = new StopState(this, statesDataSO.StoppingStateData);
            crouchState = new CrouchState(this, statesDataSO.CrouchStateData);
            //wallrunState = new WallrunState(this, statesDataSO.WallrunStateData);
            //climbState = new ClimbWallState(this, statesDataSO.ClimbStateData);
            //wallJumpState = new WallJumpState(this, statesDataSO.WallJumpStateData);
            //slideState = new SlideState(this, statesDataSO.SlidingStateData);
            //slipState = new SlipState(this, statesDataSO.SlipJumpStateData);
            //dashState = new DashState(this, statesDataSO.DashStateData);
            //flyState = new FlyState(this, statesDataSO.FlyingStateData);
            //swimState = new SwimState(this, statesDataSO.SwimStateData);
            //ledgeHangingState = new LedgeHangingState(this, statesDataSO.LedgeHangingStateData);
            //hangUpState = new LedgeHangUpState(this, statesDataSO.LedgeHangUpStateData);
            //jumpDownState = new JumpDown(this, statesDataSO.JumpDownStateData);

            StateMachine.AddAnyTransition(fallingState, new FuncPredicate(() =>
                !Sensors.IsGrounded
                ));

            //StateMachine.AddAnyTransition(slideState, new FuncPredicate(() =>
            //   !Sensors.IsValidSlope()
            //&& !groundJumpState.IsActiveState
            //&& !wallrunState.IsActiveState
            //&& !climbState.IsActiveState
            //&& !flyState.IsActiveState
            //&& !wallJumpState.IsActiveState && !swimState.IsActiveState
            //&& !Sensors.IsStepUpAhead
            //));

            //StateMachine.AddAnyTransition(swimState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Y)));
            //StateMachine.AddTransition(swimState, runState, new FuncPredicate(() => Input.GetKeyUp(KeyCode.Y)));
            //To:
            //Landing
            StateMachine.AddTransition(fallingState, landingState, new FuncPredicate(() => Sensors.IsGrounded));
            StateMachine.AddTransition(groundJumpState, landingState, new FuncPredicate(() => Sensors.IsGrounded));
            //Idle
            StateMachine.AddTransition(runState, idleState, new FuncPredicate(() => Character.MovementInput.magnitude == 0&&Character.Velocity.magnitude <= 0.01f));
            StateMachine.AddTransition(sprintState, idleState, new FuncPredicate(() => Character.MovementInput.magnitude == 0 && Character.Velocity.magnitude <= 0.01f));
            StateMachine.AddTransition(stopState, idleState, new FuncPredicate(() => Character.MovementInput.magnitude == 0 && Character.Velocity.magnitude <= 0.01f));

            StateMachine.AddTransition(crouchState, idleState, new FuncPredicate(() => !Input.GetKey(KeyCode.LeftControl)));
            //Walk
            StateMachine.AddTransition(walkState, runState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));
            StateMachine.AddTransition(runState, walkState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));
            StateMachine.AddTransition(crouchState, walkState, new FuncPredicate(() => !Input.GetKey(KeyCode.LeftControl)));
            //Run
            StateMachine.AddTransition(landingState, runState, new FuncPredicate(() => Sensors.IsGrounded));
            StateMachine.AddTransition(idleState, runState, new FuncPredicate(() => Character.MovementInput.magnitude > 0));
            StateMachine.AddTransition(sprintState, runState, new FuncPredicate(() => !Input.GetKey(KeyCode.LeftShift)));
            StateMachine.AddTransition(rollState, runState, new FuncPredicate(() => Sensors.IsGrounded));
            StateMachine.AddTransition(stopState, runState, new FuncPredicate(() => Character.MovementInput.magnitude > 0));
            StateMachine.AddTransition(crouchState, runState, new FuncPredicate(() => !Input.GetKey(KeyCode.LeftControl)));
            //GroundJump
            StateMachine.AddTransition(idleState, groundJumpState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Space)));
            StateMachine.AddTransition(runState, groundJumpState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Space)));
            StateMachine.AddTransition(sprintState, groundJumpState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Space)));
            //Sprint
            StateMachine.AddTransition(runState, sprintState, new FuncPredicate(() => Input.GetKey(KeyCode.LeftShift)&&Character.CurrentSpeed>0));
            //Roll
            StateMachine.AddTransition(idleState, rollState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.R)));
            StateMachine.AddTransition(runState, rollState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.R)));
            //Dash
            //Stopping
            StateMachine.AddTransition(runState, stopState, new FuncPredicate(() => Character.MovementInput.magnitude == 0 && Character.Velocity.magnitude >= 0.01f));
            StateMachine.AddTransition(sprintState, stopState, new FuncPredicate(() => Character.MovementInput.magnitude == 0 && Character.Velocity.magnitude >= 0.01f));
            //Crouch
            StateMachine.AddTransition(idleState, crouchState, new FuncPredicate(() => Input.GetKey(KeyCode.LeftControl)));
            StateMachine.AddTransition(runState, crouchState, new FuncPredicate(() => Input.GetKey(KeyCode.LeftControl)));
            StateMachine.AddTransition(walkState, crouchState, new FuncPredicate(() => Input.GetKey(KeyCode.LeftControl)));


            //StateMachine.AddTransition(slideState, runState, new FuncPredicate(() => Sensors.IsGrounded && (!Character.CrouchPressed || Character.Velocity.magnitude <= 2f) && Sensors.IsValidSlope()));
            //StateMachine.AddTransition(slideState, idleState, new FuncPredicate(() => Sensors.IsGrounded && (!Character.CrouchPressed || Character.Velocity.magnitude <= 2f) && Sensors.IsValidSlope()));
            //StateMachine.AddTransition(slideState, walkState, new FuncPredicate(() => Sensors.IsGrounded && (!Character.CrouchPressed || Character.Velocity.magnitude <= 2f) && Sensors.IsValidSlope()));



            //SubStateMachine.AddTransition(runState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            //StateMachine.AddTransition(idleState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            //StateMachine.AddTransition(walkState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            //StateMachine.AddTransition(walkState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            //StateMachine.AddTransition(crouchState, runState, new FuncPredicate(() => !crouchState.CanBeExecuted()));

            //StateMachine.AddTransition(crouchState, rollState, new FuncPredicate(() => rollState.CanBeExecuted()));


            //StateMachine.AddTransition(runState, dashState, new FuncPredicate(() => dashState.CanBeExecuted()));
            //StateMachine.AddTransition(dashState, runState, new FuncPredicate(() => !dashState.CanBeExecuted() && !dashState.IsActiveState));
            //StateMachine.AddTransition(dashState, sprintState, new FuncPredicate(() => sprintState.CanBeExecuted() && !dashState.IsActiveState));


            //StateMachine.AddTransition(runState, slipState, new FuncPredicate(() => slipState.CanBeExecuted()));
            //StateMachine.AddTransition(slipState, slideState, new FuncPredicate(() => !slipState.IsActiveState));

            //StateMachine.AddTransition(runState, jumpDownState, new FuncPredicate(() => jumpDownState.CanBeExecuted() && Character.MovementInput.y > 0));
            //StateMachine.AddTransition(sprintState, jumpDownState, new FuncPredicate(() => jumpDownState.CanBeExecuted() && Character.MovementInput.y > 0));
            //StateMachine.AddTransition(jumpDownState, landingState, new FuncPredicate(() => landingState.CanBeExecuted() && !jumpDownState.IsActiveState));


            StateMachine.SetState(idleState);

        }
        void Update()
        {
            currentState = StateMachine.CurrentNode.State.ToString();
            lastTransition = StateMachine.LastTransition;
            StateMachine.Update();
        }
        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        private void LateUpdate()
        {
            StateMachine.LateUpdate();
        }

    }
}
