using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
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
        // State Variables
        public CountdownTimer jumpCooldownTimer;
        [SerializeField] private StatesDataSO statesDataSO;

        LocomotionState locomotionState;
        FallingState fallingState;
        JumpState jumpState;
        CrouchState crouchState;
        RollState rollState;
        WallrunState wallrunState;
        ClimbState climbState;
        WallJumpState wallJumpState;
        SlideState slideState;

        public float SlideSlopeAngleLimit { get; private set; } = 50;

        void Start()
        {

            jumpCooldownTimer = new CountdownTimer(statesDataSO.jumpCooldownSeconds);
            TimersManager.RegisterTimer(jumpCooldownTimer);
            jumpCooldownTimer.Reset(statesDataSO.jumpCooldownSeconds);

            Sensors = GetComponent<SensorsModule>();
            Character = GetComponent<Character>();
            StateMachine = new();
            locomotionState = new LocomotionState(this,statesDataSO);
            fallingState = new FallingState(this, statesDataSO);
            jumpState = new JumpState(this,statesDataSO );
            crouchState = new CrouchState(this,statesDataSO);
            rollState = new RollState(this, statesDataSO);
            wallrunState = new WallrunState(this,statesDataSO);
            climbState = new ClimbState(this, statesDataSO);
            wallJumpState = new WallJumpState(this, statesDataSO);
            slideState = new SlideState(this, statesDataSO);


            StateMachine.AddAnyTransition(fallingState, new FuncPredicate(() => !Sensors.IsGrounded && !jumpCooldownTimer.IsRunning && !wallrunState.IsWallrunning && !climbState.IsClimbing));
            StateMachine.AddAnyTransition(slideState, new FuncPredicate(() => Sensors.IsGrounded && Mathf.Abs(Sensors.GroundAngle) > SlideSlopeAngleLimit && !climbState.IsClimbing));

            StateMachine.AddTransition(fallingState, locomotionState, new FuncPredicate(() => Sensors.IsGrounded));
            StateMachine.AddTransition(slideState, locomotionState, new FuncPredicate(() => Sensors.IsGrounded && Mathf.Abs(Sensors.GroundAngle) <= SlideSlopeAngleLimit));

            StateMachine.AddTransition(locomotionState, jumpState, new FuncPredicate(() => Character.JumpPressed && jumpCooldownTimer.IsRunning));
            StateMachine.AddTransition(jumpState, fallingState, new FuncPredicate(() => jumpCooldownTimer.IsFinished));

            StateMachine.AddTransition(locomotionState, crouchState, new FuncPredicate(() => Sensors.IsObstacleAbove || Character.CrouchPressed));
            StateMachine.AddTransition(crouchState, locomotionState, new FuncPredicate(() => !Sensors.IsObstacleAbove && !Character.CrouchPressed));

            StateMachine.AddTransition(locomotionState, rollState, new FuncPredicate(() => Character.EvadePressed));
            StateMachine.AddTransition(rollState, locomotionState, new FuncPredicate(() => !rollState.IsRolling));
            StateMachine.AddTransition(crouchState, rollState, new FuncPredicate(() => Character.EvadePressed));
            StateMachine.AddTransition(rollState, crouchState, new FuncPredicate(() => !rollState.IsRolling && (Sensors.IsObstacleAbove || Character.CrouchPressed)));


            StateMachine.AddTransition(jumpState, wallrunState, new FuncPredicate(() => wallrunState.CanWallRun()));
            StateMachine.AddTransition(wallJumpState, wallrunState, new FuncPredicate(() => wallrunState.CanWallRun()));
            StateMachine.AddTransition(wallrunState, fallingState, new FuncPredicate(() => !wallrunState.CanWallRun()));

            StateMachine.AddTransition(jumpState, climbState, new FuncPredicate(() => climbState.CanClimbWall()));
            StateMachine.AddTransition(climbState, fallingState, new FuncPredicate(() => !climbState.CanClimbWall()));

            StateMachine.AddTransition(climbState, wallJumpState, new FuncPredicate(() => Character.JumpPressed && jumpCooldownTimer.IsRunning));
            StateMachine.AddTransition(wallrunState, wallJumpState, new FuncPredicate(() => Character.JumpPressed && jumpCooldownTimer.IsRunning));

            

            

            StateMachine.SetState(locomotionState);

        }

        // Update is called once per frame
        void Update()
        {
            currentState = StateMachine.CurrentNode.State.ToString();
            if (Character.JumpPressed)
            {
                OnJump();
            }

            StateMachine.Update();
        }
        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        private void OnJump()
        {
            if ((Sensors.IsGrounded || wallrunState.IsWallrunning || climbState.IsClimbing) && !jumpCooldownTimer.IsRunning)
            {
                jumpCooldownTimer.Start();
            }

        }

    }
}
