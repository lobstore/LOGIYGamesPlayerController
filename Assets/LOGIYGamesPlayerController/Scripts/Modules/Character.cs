using System.Collections.Generic;
using UnityEngine;
using LOGIYGames.Timers;
namespace LOGIYGames.CharacterCore
{

    [RequireComponent(typeof(SensorsModule))]
    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoModuleBase, IControllable
    {

        [Header("References")]
        [SerializeField] InputReader InputReader;
        private CharacterController CController = null;
        #region VelocityVariables

        /// <summary>
        /// Gets or sets the deceleration value.
        /// </summary>
        /// <remarks>
        /// Deceleration does not work if the <see cref="useInertia"/> variable is set to <c>false</c>.
        /// </remarks>
        public float Deceleration { get; set; }

        /// <summary>
        /// Gets or sets the acceleration value.
        /// </summary>
        /// <remarks>
        /// Acceleration does not work if the <see cref="useInertia"/> variable is set to <c>false</c>.
        /// </remarks>
        public float Acceleration { get; set; }

        public float TotalSpeedMultiplier => InternalSpeedMultiplier * ExternalSpeedMultiplier;
        /// <summary>
        /// Gets or sets the external speed multiplier.
        /// </summary>
        /// <remarks>
        /// This multiplier can be used to adjust the movement speed in specific conditions, such as when moving on ice or snow.
        /// </remarks>
        public float ExternalSpeedMultiplier { get; set; } = 1f;
        /// <summary>
        /// Gets or sets the internal speed multiplier.
        /// </summary>
        /// <remarks>
        /// This multiplier can be used to adjust the movement speed in base conditions, such as when moving walk or sprint.
        /// </remarks>
        public float InternalSpeedMultiplier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [field: SerializeField] public float BaseSpeed { get; set; }
        public float CurrentSpeed => TotalSpeedMultiplier * BaseSpeed;

        public Vector3 HorizontalVelocity { get => horizontalVelocity; set => horizontalVelocity = value; }

        private Vector3 horizontalVelocity;
        #endregion
        [field: SerializeField] public float Height { get; set; }
        public bool IsUnderPlayerControl {  get; private set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;

        [field: SerializeField] public Transform CinemachineCameraLookAtTransform { get; set; }
        [field: SerializeField] public Transform CinemachineCameraFollowTransform { get; set; }

        #region Inputs
        public bool JumpPressed { get; set; }
        public bool EvadePressed { get; set; }
        public bool AttackPressed { get; set; }
        public bool CrouchPressed { get; private set; }
        public bool SprintPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool BlockPressed { get; private set; }
        public bool FocusPressed { get; private set; }
        public Vector2 MovementInput {  get; private set; }

        #endregion
        [field: SerializeField] public Transform Target {  get; private set; }
        private void Awake()
        {
            InputReader.CharacterInputsEnable = true;
            InputReader.CameraInputsEnable = true;
            CController = GetComponent<CharacterController>();
            Height = CController.height;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
        }

        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            SmoothHeightChanging();

        }
        public void HandleInputs()
        {
            JumpPressed = InputReader.JumpPressed;
            CrouchPressed = InputReader.CrouchPressed;
            AttackPressed = InputReader.AttackPressed;
            BlockPressed = InputReader.BlockPressed;
            SprintPressed = InputReader.SprintPressed;
            EvadePressed = InputReader.EvadePressed;
            MovementInput = InputReader.MovementInput;
        }
        private void SmoothHeightChanging()
        {
            if (Height == CController.height) return;
            if (CController.height > Height + 0.01f || CController.height < Height - 0.01f)
            {
                CController.height = Mathf.Lerp(CController.height, Height, HeightChangingSmoothTime * Time.deltaTime);
            }
            else
            {
                CController.height = Height;
            }

        }

        /// <summary>
        /// Rotate object to specified direction with specified speed
        /// </summary>
        /// <param name="desiredDirection">Direction to rotate</param>
        /// <param name="turnSmoothTime">Turn speed, instant turn if 0, if !=0, method should be used in update</param>
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            desiredDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            Rotate(targetRotation, turnSmoothTime);
        }
        public void RotateToPosition(Vector3 position)
        {
            Vector3 desiredDirection = position - transform.position;
            RotateToDirection(desiredDirection.normalized);
        }
        public void Rotate(Quaternion targetRotation, float turnSmoothTime = 0)
        {
            if (turnSmoothTime != 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSmoothTime);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
        public void ResetMotion()
        {
            horizontalVelocity = Vector3.zero;
            Acceleration = 0;
            Deceleration = 0;
            InternalSpeedMultiplier = 0;
        }

        public void OnControlGained()
        {
            IsUnderPlayerControl = true;
        }

        public void OnControlLost()
        {
            JumpPressed = false;
            CrouchPressed = false;
            AttackPressed = false;
            BlockPressed = false;
            SprintPressed = false;
            MovementInput = Vector2.zero;
            IsUnderPlayerControl = false;
        }

        public void EnableControl()
        {
           
        }

        public void DisableControl()
        {
           
        }
    }
}