using LOGIYGames;
using UnityEngine;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(SensorsModule))]
    [DefaultExecutionOrder(-1)]
    public class CrouchActionContext : LocomotionActionContext
    {

        [SerializeField] float crouchSpeed = 0.3f;
        [Header("Movement Settings")]
        [SerializeField] private float crouchHeightMultiplier = 0.5f;
        [Header("Component References")]
        private CharacterController characterController;

        public bool IsCrouching => Sensors.IsObstacleAbove || IsCrouchingPressed;
        private bool IsCrouchingPressed;
        public float CrouchHeight { get; private set; }
        public float StandingHeight { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
            InitializeHeightValues();
        }

        private void Update()
        {
            IsCrouchingPressed = Character.CrouchPressed;
        }

        private void InitializeComponents()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void InitializeHeightValues()
        {
            StandingHeight = characterController.height;
            CrouchHeight = StandingHeight * crouchHeightMultiplier;
        }

        public override void EnterState()
        {
            base.EnterState();
            Character.Height = CrouchHeight;
            InternalSpeedMultiplier = crouchSpeed;
        }
        public override void ExitState()
        {
            base.ExitState();
            Character.Height = StandingHeight;
        }

    }
}