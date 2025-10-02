using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System.Collections.Generic;
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
        [SerializeField] private float jumpCooldown;
        void Start()
        {

            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            TimersManager.RegisterTimer(jumpCooldownTimer);
            jumpCooldownTimer.Reset(jumpCooldown);

            Sensors = GetComponent<SensorsModule>();
            Character = GetComponent<Character>();
            StateMachine = new();
            var locomotionState = new LocomotionState(this, 7, 4, MotionType.CharacterController);
            var fallingState = new FallingState(this, 1, 1, 1,  MotionType.CharacterController);
            var jumpState = new JumpState(this, 5, 5, MotionType.CharacterController);
            var crouchState = new CrouchState(this, 7, 4, MotionType.CharacterController);
            var rollState = new RollState(this, MotionType.AnimatorController);
            var wallrunState= new WallrunState(this, 7, 4, MotionType.CharacterController);

            StateMachine.AddAnyTransition(fallingState, new FuncPredicate(()=>!Sensors.IsGrounded && !jumpCooldownTimer.IsRunning&&!wallrunState.IsWallrunning) );
            StateMachine.AddTransition(fallingState, locomotionState, new FuncPredicate(() => Sensors.IsGrounded));

            StateMachine.AddTransition(locomotionState, jumpState, new FuncPredicate(() => Character.JumpPressed && jumpCooldownTimer.IsRunning ));
            StateMachine.AddTransition(jumpState, fallingState, new FuncPredicate(() => jumpCooldownTimer.IsFinished ));

            StateMachine.AddTransition(locomotionState, crouchState, new FuncPredicate(() => Sensors.IsObstacleAbove||Character.CrouchPressed));
            StateMachine.AddTransition(crouchState, locomotionState, new FuncPredicate(() => !Sensors.IsObstacleAbove&&!Character.CrouchPressed));

            StateMachine.AddTransition(locomotionState, rollState, new FuncPredicate(() => Character.EvadePressed));
            StateMachine.AddTransition(rollState, locomotionState, new FuncPredicate(() => !rollState.IsRolling));
            StateMachine.AddTransition(crouchState, rollState, new FuncPredicate(() => Character.EvadePressed));
            StateMachine.AddTransition(rollState, crouchState, new FuncPredicate(() => !rollState.IsRolling && (Sensors.IsObstacleAbove || Character.CrouchPressed)));


            StateMachine.AddTransition(jumpState, wallrunState, new FuncPredicate(() => wallrunState.CanWallRun() ));
            StateMachine.AddTransition(wallrunState, fallingState, new FuncPredicate(() => !wallrunState.CanWallRun() ));

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
            if (Sensors.IsGrounded && !jumpCooldownTimer.IsRunning)
            {
                jumpCooldownTimer.Start();
            }

        }

    }
}
