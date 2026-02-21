using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class MovementStateData
    {
        public string StateName;
        [Space]
        public float TurnSmoothTime = 2;
        public float Acceleration = 6;
        public float Deceleration = 6;
        public float Speed = 0;
    }

    /// <summary>
    /// Base class for states with timer support (duration + cooldown)
    /// </summary>
    [Serializable]
    public class TimedMovementStateData : MovementStateData
    {
        [Header("Timing")]
        [Tooltip("Minimum duration in this state before can transition out")]
        public float Duration = 0.5f;

        [Tooltip("Cooldown before can re-enter this state")]
        public float Cooldown = 0.2f;
    }

    [Serializable]
    public class JumpStateData : TimedMovementStateData
    {
        [Header("Jump Forces")]
        public float PlanarJumpForce = 5f;
        public float VerticalJumpForce = 10f;
    }

    [Serializable]
    public class RollStateData : TimedMovementStateData
    {
        [Header("Roll Forces")]
        public float PlanarForce = 5f;
        public float VerticalForce = 2f;
    }

    [Serializable]
    public class LandingStateData : TimedMovementStateData
    {
    }

    [CreateAssetMenu(fileName = "StatesDataSO", menuName = "MovementStateMachine/StatesDataSO")]
    public class MovementStatesDataSO : ScriptableObject
    {
        [Header("Idle State")]
        public MovementStateData IdleStateData;

        [Space]
        [Header("Walk State")]
        public MovementStateData WalkStateData;

        [Space]
        [Header("Run State")]
        public MovementStateData RunStateData;

        [Space]
        [Header("Crouch State")]
        public MovementStateData CrouchStateData;

        [Space]
        [Header("Falling State")]
        public MovementStateData FallingStateData;

        [Space]
        [Header("Landing State")]
        public LandingStateData LandingStateData;

        [Space]
        [Header("Roll State")]
        public RollStateData RollStateData;

        [Space]
        [Header("GroundJump State")]
        public JumpStateData GroundJumpStateData;

        [Space]
        [Header("Sprint State")]
        public MovementStateData SprintStateData;

        [Space]
        [Header("Stopping State")]
        public MovementStateData StoppingStateData;
    }
}
