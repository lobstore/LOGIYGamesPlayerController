using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class StateData
    {
        public string StateName = "";
        [Space]
        public float TurnSmothTime = 2;
        public float Acceleration = 6;
        public float Deceleration = 6;
        public float Speed;
        [Space]
        public float StaminaCost;
        [Space]
        public MotionType MotionType;
        public AnimationCurve AnimationCurve = null;
        [Space]
        public bool RotateByRootMotionOnly = false;
        public bool HandlingSlope = true;
        public bool UseProjectionOnPlane = true;
    }
    [Serializable]
    public class TimedCooldownStateData : StateData
    {
        public float ActiveStateTime;
        public float CooldownStateTime;
    }
    [Serializable]
    public class JumpStateData : TimedCooldownStateData
    {
        public float PlanarJumpForce;
        public float VerticalJumpForce;
    }
    [CreateAssetMenu(fileName = "StatesDataSO", menuName = "MovementStateMachine/StatesDataSO")]
    public class StatesDataSO : ScriptableObject
    {
        [Header("Idle State")]

        public StateData IdleStateData;

        [Header("Walk State")]

        public StateData WalkStateData;

        [Header("Run State")]

        public StateData RunStateData;

        [Space]
        [Header("Sprint State")]

        public StateData SprintStateData;

        [Space]
        [Header("Crouch State")]

        public StateData CrouchStateData;

        [Space]
        [Header("Falling State")]

        public StateData FallingStateData;

        [Space]
        [Header("Landing State")]

        public TimedCooldownStateData LandingStateData;

        [Space]
        [Header("Wallrun State")]

        public StateData WallrunStateData;

        [Space]
        [Header("Climb State")]

        public StateData ClimbStateData;

        [Space]
        [Header("GroundJump State")]

        public JumpStateData GroundJumpStateData;

        [Space]
        [Header("Roll State")]

        public JumpStateData RollStateData;

        [Space]
        [Header("Walljump State")]

        public JumpStateData WallJumpStateData;

        [Space]
        [Header("Slipjump State")]

        public JumpStateData SlipJumpStateData;

        [Space]
        [Header("Slide State")]

        public StateData SlidingStateData;

        [Space]
        [Header("Dash State")]

        public JumpStateData DashStateData;

        [Space]
        [Header("Fly State")]
        public StateData FlyingStateData;

        [Space]
        [Header("Swim State")]
        public StateData SwimStateData;

        [Space]
        [Header("Ledge Hanging State")]
        public StateData LedgeHangingStateData;

        [Space]
        [Header("HangUp State")]
        public TimedCooldownStateData LedgeHangUpStateData;

        [Space]
        [Header("Jump Down State")]
        public JumpStateData JumpDownStateData;

        [Space]
        [Header("Jump Down State")]
        public StateData AimStateData;
    }
}
