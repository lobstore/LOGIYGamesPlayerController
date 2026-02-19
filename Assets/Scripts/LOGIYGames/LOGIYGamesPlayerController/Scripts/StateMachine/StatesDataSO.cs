using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class StateData
    {
        public string StateName;
        [Space]
        public float TurnSmothTime = 2;
        public float Acceleration = 6;
        public float Deceleration = 6;
        public float Speed = 0;

      
    }

    [Serializable]
    public class JumpStateData : StateData
    {
        public float PlanarJumpForce;
        public float VerticalJumpForce;
    }
    [CreateAssetMenu(fileName = "StatesDataSO", menuName = "MovementStateMachine/StatesDataSO")]
    public class StatesDataSO : ScriptableObject
    {
        [Header("Idle State")]

        public StateData IdleStateData;
        [Space]
        [Header("Walk State")]

        public StateData WalkStateData;
        [Space]
        [Header("Run State")]

        public StateData RunStateData;

        [Space]
        [Header("Crouch State")]

        public StateData CrouchStateData;

        [Space]
        [Header("Falling State")]

        public StateData FallingStateData;

        [Space]
        [Header("Landing State")]

        public StateData LandingStateData;

        [Space]
        [Header("Roll State")]

        public JumpStateData RollStateData;

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
        [Header("Sprint State")]

        public StateData SprintStateData;
        [Space]
        [Header("Stopping State")]

        public StateData StoppingStateData;

    }
}
