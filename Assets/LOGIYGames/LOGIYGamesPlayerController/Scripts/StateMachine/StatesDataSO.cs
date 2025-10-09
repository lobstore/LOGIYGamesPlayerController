using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class StateData
    {
        public string StateName = "";
        public AnimationCurve AnimationCurve = null;
        public MotionType MotionType;
        public float TurnSmothTime;
        public float Acceleration;
        public float Deceleration;
        public float Speed;
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
        [Header("Locomotion State")]

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
    }
}
