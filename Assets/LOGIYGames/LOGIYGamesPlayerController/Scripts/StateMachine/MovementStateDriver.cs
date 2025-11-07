using LOGIYGames.CharacterCore;
using UnityEditor.Experimental;
using UnityEngine;

namespace LOGIYGames
{
    public class MovementStateDriver : MonoBehaviour
    {
        StateMachine StateMachine;
        public Character Character;
        public StaminaModel StaminaModel;
        public CharacterGravityModule GravityModule;
        public SensorsModule Sensors;
        public Animator Animator;
        public GenericControllerWrapper ControllerWrapper;

        string currentState;
        [SerializeField] private StatesDataSO statesDataSO;

        IdleState idleState;
        WalkState walkState;
        RunState runState;
        SprintState sprintState;

        FallingState fallingState;
        LandingState landingState;
        GroundJumpState groundJumpState;

        CrouchState crouchState;
        RollState rollState;

        WallrunState wallrunState;
        ClimbWallState climbState;
        WallJumpState wallJumpState;

        SlideState slideState;
        SlipState slipState;

        DashState dashState;
        FlyState flyState;
        SwimState swimState;
        LedgeHangingState ledgeHangingState;
        LedgeHangUpState hangUpState;
        JumpDown jumpDownState;

        AimState aimState;
        
        void Start()
        {
            StateMachine = new();
            idleState = new IdleState(this, statesDataSO.IdleStateData);
            walkState = new WalkState(this, statesDataSO.WalkStateData);
            runState = new RunState(this, statesDataSO.RunStateData);
            sprintState = new SprintState(this, statesDataSO.SprintStateData);
            fallingState = new FallingState(this, statesDataSO.FallingStateData);
            landingState = new LandingState(this, statesDataSO.LandingStateData);
            groundJumpState = new GroundJumpState(this, statesDataSO.GroundJumpStateData);
            crouchState = new CrouchState(this, statesDataSO.CrouchStateData);
            rollState = new RollState(this, statesDataSO.RollStateData);
            wallrunState = new WallrunState(this, statesDataSO.WallrunStateData);
            climbState = new ClimbWallState(this, statesDataSO.ClimbStateData);
            wallJumpState = new WallJumpState(this, statesDataSO.WallJumpStateData);
            slideState = new SlideState(this, statesDataSO.SlidingStateData);
            slipState = new SlipState(this, statesDataSO.SlipJumpStateData);
            dashState = new DashState(this, statesDataSO.DashStateData);
            flyState = new FlyState(this, statesDataSO.FlyingStateData);
            swimState = new SwimState(this, statesDataSO.SwimStateData);
            ledgeHangingState = new LedgeHangingState(this, statesDataSO.LedgeHangingStateData);
            hangUpState = new LedgeHangUpState(this, statesDataSO.LedgeHangUpStateData);
            jumpDownState = new JumpDown(this, statesDataSO.JumpDownStateData);
            aimState = new AimState(this, statesDataSO.AimStateData);

            StateMachine.AddAnyTransition(fallingState, new FuncPredicate(() =>
                !Sensors.IsGrounded
                && !groundJumpState.IsActiveState
                && !wallrunState.IsActiveState
                && !climbState.IsActiveState
                && !flyState.IsActiveState
                && !wallJumpState.IsActiveState
                && !swimState.IsActiveState
                && !ledgeHangingState.IsActiveState
                && !hangUpState.IsActiveState
                && !jumpDownState.IsActiveState
                && !slipState.IsActiveState
                ));

            StateMachine.AddAnyTransition(slideState, new FuncPredicate(() =>
               !Sensors.IsValidSlope()
            && !groundJumpState.IsActiveState
            && !wallrunState.IsActiveState
            && !climbState.IsActiveState
            && !flyState.IsActiveState
            && !wallJumpState.IsActiveState && !swimState.IsActiveState
            && !Sensors.IsStepAhead
            ));

            StateMachine.AddAnyTransition(swimState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Y)));
            StateMachine.AddTransition(swimState, runState, new FuncPredicate(() => Input.GetKeyUp(KeyCode.Y)));

            StateMachine.AddTransition(idleState, runState, new FuncPredicate(() => Character.MovementInput.magnitude>0));
            StateMachine.AddTransition(walkState, runState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));
            StateMachine.AddTransition(runState, walkState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Z)));
            StateMachine.AddTransition(runState, idleState, new FuncPredicate(() => Character.MovementInput.magnitude == 0));



            StateMachine.AddTransition(fallingState, landingState, new FuncPredicate(() => Sensors.IsGrounded));
            StateMachine.AddTransition(landingState, runState, new FuncPredicate(() => Sensors.IsGrounded && !landingState.IsActiveState));
            
            StateMachine.AddTransition(slideState, runState, new FuncPredicate(() => Sensors.IsGrounded && (!Character.CrouchPressed||Character.Velocity.magnitude<=2f) && Sensors.IsValidSlope()));
            StateMachine.AddTransition(slideState, idleState, new FuncPredicate(() => Sensors.IsGrounded && (!Character.CrouchPressed||Character.Velocity.magnitude<=2f) && Sensors.IsValidSlope()));
            StateMachine.AddTransition(slideState, walkState, new FuncPredicate(() => Sensors.IsGrounded && (!Character.CrouchPressed||Character.Velocity.magnitude<=2f) && Sensors.IsValidSlope()));

            StateMachine.AddTransition(groundJumpState, fallingState, new FuncPredicate(() => !groundJumpState.IsActiveState && !Sensors.IsGrounded));
            StateMachine.AddTransition(groundJumpState, landingState, new FuncPredicate(() => !groundJumpState.IsActiveState && landingState.CanBeExecuted()));

            StateMachine.AddTransition(runState, groundJumpState, new FuncPredicate(() => groundJumpState.CanBeExecuted()));
            StateMachine.AddTransition(sprintState, groundJumpState, new FuncPredicate(() => groundJumpState.CanBeExecuted()));
            StateMachine.AddTransition(idleState, groundJumpState, new FuncPredicate(() => groundJumpState.CanBeExecuted()));


            //StateMachine.AddTransition(runState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            StateMachine.AddTransition(idleState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            StateMachine.AddTransition(walkState, crouchState, new FuncPredicate(() => crouchState.CanBeExecuted()));
            StateMachine.AddTransition(crouchState, runState, new FuncPredicate(() => !crouchState.CanBeExecuted()));

            StateMachine.AddTransition(runState, rollState, new FuncPredicate(() => rollState.CanBeExecuted()));
            StateMachine.AddTransition(rollState, runState, new FuncPredicate(() => !rollState.IsActiveState));
            StateMachine.AddTransition(crouchState, rollState, new FuncPredicate(() => rollState.CanBeExecuted()));


            //StateMachine.AddTransition(groundJumpState, wallrunState, new FuncPredicate(() => wallrunState.CanBeExecuted()));
            //StateMachine.AddTransition(wallJumpState, wallrunState, new FuncPredicate(() => wallrunState.CanBeExecuted() && !wallJumpState.IsActiveState));
            //StateMachine.AddTransition(wallJumpState, landingState, new FuncPredicate(() => !wallJumpState.IsActiveState && Sensors.IsGrounded));
            //StateMachine.AddTransition(wallrunState, fallingState, new FuncPredicate(() => !wallrunState.CanBeExecuted()));

            //StateMachine.AddTransition(groundJumpState, climbState, new FuncPredicate(() => climbState.CanBeExecuted()));
            //StateMachine.AddTransition(climbState, fallingState, new FuncPredicate(() => !climbState.CanBeExecuted()));

            //StateMachine.AddTransition(climbState, wallJumpState, new FuncPredicate(() => wallJumpState.CanBeExecuted()));
            //StateMachine.AddTransition(wallrunState, wallJumpState, new FuncPredicate(() => wallJumpState.CanBeExecuted()));


            StateMachine.AddTransition(runState, dashState, new FuncPredicate(() => dashState.CanBeExecuted()));
            StateMachine.AddTransition(dashState, runState, new FuncPredicate(() => !dashState.CanBeExecuted() && !dashState.IsActiveState));
            StateMachine.AddTransition(dashState, sprintState, new FuncPredicate(() => sprintState.CanBeExecuted() && !dashState.IsActiveState));
            StateMachine.AddTransition(sprintState, runState, new FuncPredicate(() => !sprintState.CanBeExecuted()));


            StateMachine.AddTransition(runState, slipState, new FuncPredicate(() => slipState.CanBeExecuted()));
            StateMachine.AddTransition(slipState, slideState, new FuncPredicate(() => !slipState.IsActiveState));


            StateMachine.AddTransition(runState, flyState, new FuncPredicate(() => flyState.CanBeExecuted()));
            StateMachine.AddTransition(flyState, runState, new FuncPredicate(() => !flyState.CanBeExecuted()));

            StateMachine.AddTransition(idleState, aimState, new FuncPredicate(() => aimState.CanBeExecuted()));
            StateMachine.AddTransition(aimState, idleState, new FuncPredicate(() => !aimState.CanBeExecuted()));


            //StateMachine.AddTransition(groundJumpState, ledgeHangingState, new FuncPredicate(() => Sensors.ForeheadFrontHit.collider?.tag == "Ledge"));
            //StateMachine.AddTransition(ledgeHangingState, fallingState, new FuncPredicate(() => Character.JumpPressed));


            StateMachine.AddTransition(climbState, hangUpState, new FuncPredicate(() => hangUpState.CanBeExecuted() && Character.MovementInput.y > 0));
            //StateMachine.AddTransition(hangUpState, runState, new FuncPredicate(() => !hangUpState.IsActiveState && Sensors.IsGrounded));

            StateMachine.AddTransition(runState, jumpDownState, new FuncPredicate(() => jumpDownState.CanBeExecuted() && Character.MovementInput.y > 0));
            StateMachine.AddTransition(sprintState, jumpDownState, new FuncPredicate(() => jumpDownState.CanBeExecuted() && Character.MovementInput.y > 0));
            StateMachine.AddTransition(jumpDownState, landingState, new FuncPredicate(() => landingState.CanBeExecuted() && !jumpDownState.IsActiveState));




            StateMachine.SetState(idleState);

        }
        // Update is called once per frame
        void Update()
        {
            currentState = StateMachine.CurrentNode.State.ToString();

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
