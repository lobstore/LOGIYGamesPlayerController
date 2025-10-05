using Unity.VisualScripting;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "StatesDataSO", menuName = "MovementStateMachine/StatesDataSO")]
    public class StatesDataSO : ScriptableObject
    {
        [Header("Locomotion State")]

        public float walkSpeed;
        public float runSpeed;
        public float sprintSpeed;
        public float locomotonAcceleration;
        public float locomotonDeceleration;
        public float turnSmoothingTimeLocomotion;
        public MotionType locomotionMotionType;
        public AnimationCurve locomotionCurve;

        [Space]
        [Header("Crouch State")]

        public float crouchSpeed;
        public float crouchAcceleration;
        public float crouchDeceleration;
        public float turnSmoothingTimeCrouch;
        public MotionType crouchMotionType;

        [Space]
        [Header("Falling State")]

        public float airSpeed;
        public float airAcceleration;
        public float airDeceleration;
        public float turnSmoothingTimeFalling;
        public MotionType fallingMotionType;

        [Space]
        [Header("Wallrun State")]

        public float wallrunSpeed;
        public float wallrunAcceleration;
        public float wallrunDeceleration;
        public float wallrunGravityMultiplier;
        public bool useWallclippingWallrun;
        public MotionType wallrunMotionType;

        [Space]
        [Header("Climb State")]

        public float climbSpeed;
        public float climbAcceleration;
        public float climbDeceleration;
        public bool useWallclippingClimb;
        public MotionType climbMotionType;

        [Space]
        [Header("Jump State")]

        public float jumpCooldownSeconds;
        public float verticalJumpForce;
        public float planarJumpForce;
        public MotionType jumpMotionType;

        [Space]
        [Header("Roll State")]

        public float rollJumpForce;
        public MotionType rollMotionType;

        [Space]
        [Header("Walljump State")]

        public float verticalWallrunJumpForce;
        public float planarWallrunJumpForce;
        public MotionType walljumpMotionType;

        [Space]
        [Header("Slipjump State")]

        public float slipJumpForce;
        public MotionType slipMotionType;

        [Space]
        [Header("Slide State")]

        public float slideSpeed;

        [Space]
        [Header("Grounded State")]
        public float slopeAffectMultiplier;

    }
}
