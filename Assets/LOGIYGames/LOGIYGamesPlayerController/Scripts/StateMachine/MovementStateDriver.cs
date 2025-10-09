using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class MovementStateDriver : MonoBehaviour
    {
        StateMachine StateMachine;
        Character Character;
        CharacterGravityModule GravityModule;
        SensorsModule Sensors;

        string currentState;
        [SerializeField] private StatesDataSO statesDataSO;

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

        public float SlideSlopeAngleLimit { get; private set; } = 50;

        void Start()
        {
            Sensors = GetComponent<SensorsModule>();
            Character = GetComponent<Character>();
            StateMachine = new();
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

            StateMachine.AddAnyTransition(fallingState, new FuncPredicate(() => 
                !Sensors.IsGrounded 
                && !groundJumpState.IsActiveState
                && !wallrunState.IsActiveState 
                && !climbState.IsActiveState 
                && !flyState.IsActiveState 
                && !wallJumpState.IsActiveState 
                && !swimState.IsActiveState
                && !ledgeHangingState.IsActiveState
                ));

            StateMachine.AddAnyTransition(slideState, new FuncPredicate(() => 
            Sensors.IsGrounded 
            && Mathf.Abs(Sensors.GroundAngle) > SlideSlopeAngleLimit 
            && !groundJumpState.IsActiveState 
            && !wallrunState.IsActiveState 
            && !climbState.IsActiveState 
            && !flyState.IsActiveState 
            && !wallJumpState.IsActiveState&& !swimState.IsActiveState)
                );

            StateMachine.AddAnyTransition(swimState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Y)));
            StateMachine.AddTransition(swimState, runState, new FuncPredicate(() => Input.GetKeyUp(KeyCode.Y)));

            StateMachine.AddTransition(fallingState, landingState, new FuncPredicate(() => Sensors.IsGrounded));
            StateMachine.AddTransition(landingState, runState, new FuncPredicate(() => Sensors.IsGrounded && !landingState.IsActiveState));
            StateMachine.AddTransition(slideState, runState, new FuncPredicate(() => Sensors.IsGrounded && Mathf.Abs(Sensors.GroundAngle) <= SlideSlopeAngleLimit));

            StateMachine.AddTransition(runState, groundJumpState, new FuncPredicate(() => groundJumpState.CanBeExecuted() && Sensors.IsGrounded && Character.JumpPressed));
            StateMachine.AddTransition(sprintState, groundJumpState, new FuncPredicate(() => groundJumpState.CanBeExecuted() && Sensors.IsGrounded && Character.JumpPressed));
            StateMachine.AddTransition(groundJumpState, fallingState, new FuncPredicate(() => !groundJumpState.IsActiveState && !Sensors.IsGrounded));
            StateMachine.AddTransition(groundJumpState, landingState, new FuncPredicate(() => !groundJumpState.IsActiveState && Sensors.IsGrounded));


            StateMachine.AddTransition(runState, crouchState, new FuncPredicate(() => (Sensors.IsObstacleAbove || Character.CrouchPressed) && Character.MovementInput.y == 0));
            StateMachine.AddTransition(crouchState, runState, new FuncPredicate(() => !Sensors.IsObstacleAbove && !Character.CrouchPressed));

            StateMachine.AddTransition(runState, rollState, new FuncPredicate(() => Character.EvadePressed && rollState.CanBeExecuted()));
            StateMachine.AddTransition(rollState, runState, new FuncPredicate(() => !rollState.IsActiveState));
            StateMachine.AddTransition(crouchState, rollState, new FuncPredicate(() => Character.EvadePressed));
            StateMachine.AddTransition(rollState, crouchState, new FuncPredicate(() => !rollState.IsActiveState && (Sensors.IsObstacleAbove || Character.CrouchPressed)));


            StateMachine.AddTransition(groundJumpState, wallrunState, new FuncPredicate(() => wallrunState.CanWallRun()));
            StateMachine.AddTransition(wallJumpState, wallrunState, new FuncPredicate(() => !wallJumpState.IsActiveState && wallrunState.CanWallRun()));
            StateMachine.AddTransition(wallJumpState, landingState, new FuncPredicate(() => !wallJumpState.IsActiveState && Sensors.IsGrounded));
            StateMachine.AddTransition(wallrunState, fallingState, new FuncPredicate(() => !wallrunState.CanWallRun()));

            StateMachine.AddTransition(groundJumpState, climbState, new FuncPredicate(() => climbState.CanClimbWall()));
            StateMachine.AddTransition(climbState, fallingState, new FuncPredicate(() => !climbState.CanClimbWall()));

            StateMachine.AddTransition(climbState, wallJumpState, new FuncPredicate(() => wallJumpState.CanBeExecuted()&&Character.JumpPressed));
            StateMachine.AddTransition(wallrunState, wallJumpState, new FuncPredicate(() => wallJumpState.CanBeExecuted() && Character.JumpPressed));


            StateMachine.AddTransition(runState, dashState, new FuncPredicate(() => dashState.CanBeExecuted()&& Character.SprintPressed && Character.MovementInput.magnitude > 0));
            StateMachine.AddTransition(dashState, runState, new FuncPredicate(() => !dashState.IsActiveState && (!Character.SprintPressed || Character.MovementInput.magnitude == 0)));
            StateMachine.AddTransition(dashState, sprintState, new FuncPredicate(() => !dashState.IsActiveState && Character.SprintPressed ));
            StateMachine.AddTransition(sprintState, runState, new FuncPredicate(() => !Character.SprintPressed || Character.MovementInput.magnitude == 0));


            StateMachine.AddTransition(runState, slipState, new FuncPredicate(() => slipState.CanBeExecuted() && Character.CrouchPressed && Character.MovementInput.y > 0));
            StateMachine.AddTransition(slipState, runState, new FuncPredicate(() => !slipState.IsActiveState));


            StateMachine.AddTransition(runState, flyState, new FuncPredicate(() => Character.BlockPressed));
            StateMachine.AddTransition(flyState, runState, new FuncPredicate(() => !Character.BlockPressed));


            StateMachine.AddTransition(groundJumpState, ledgeHangingState, new FuncPredicate(() => Sensors.ForeheadFrontHit.collider?.tag == "Ledge"));
            StateMachine.AddTransition(ledgeHangingState, fallingState, new FuncPredicate(() => Character.JumpPressed));




            StateMachine.SetState(runState);

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


    }
}
