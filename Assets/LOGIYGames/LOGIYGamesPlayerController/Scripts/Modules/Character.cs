using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.CharacterCore
{
    public class Character : MonoModuleBase, IControllable
    {
        [Header("References")]
        public IMovementInputReader Input { get; set; }

        public Transform Target;

        public ControllerWrapperBase CController { get; set; }
        [SerializeField] private SensorsModule sensors;
        // TODO Make Builder
        public IMovementStrategy CurrentMovementStrategy { get; set; }
        public IRotationStrategy CurrentRotationStrategy { get; set; }

        public IMovementStrategy DefaultMovementStrategy { get; set; }
        public IRotationStrategy DefaultRotaionStrategy { get; set; }

        public UnityEvent OnJump = new();
        public UnityEvent OnRoll = new();
        public UnityEvent OnDash = new();
        public UnityEvent OnBackTurn = new();

        public bool IsFalling { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsGrounded { get => CController.IsGrounded; }
        public bool IsSliding { get; set; }


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

        /// <summary>
        /// Gets or sets the internal speed multiplier.
        /// </summary>
        /// <remarks>
        /// This multiplier can be used to adjust the movement speed in base conditions, such as when moving walk or sprint.
        /// </remarks>
        public float SpeedMultiplier { get; set; }

        public float TurnSmoothTime { get; set; } = 5f;

        [field: SerializeField] public float BaseSpeed { get; set; }
        public float CurrentSpeed => SpeedMultiplier * BaseSpeed * Input.MovementInput.magnitude;

        public Vector3 Velocity { get => velocity; set => velocity = value; }

        public float JumpVerticalForce { get; set; }
        public float JumpPlanarForce { get; set; }

        private Vector3 velocity;
        private float deltaYaw;
        private float lastYRotation;
        private float currentYRotation;


        #endregion

        #region Height Properties

        [field: SerializeField] public float Height { get; set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;

        #endregion

        #region Camera References

        [field: SerializeField] public Transform CinemachineCameraLookAtTransform { get; set; }
        [field: SerializeField] public Transform CinemachineCameraFollowTransform { get; set; }
        public float DeltaYaw { get => deltaYaw; set => deltaYaw = value; }

        #endregion

        private void Start()
        {
            // TODO Make ICBFollowable abstraction to change follow target
            CameraManager.Instance.SetTargetTo(CinemachineCameraFollowTransform, CinemachineCameraLookAtTransform);
            CController.Height = Height;
            CController.Center = new Vector3(0, Height / 2.0f, 0);


        }

        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            SmoothHeightChanging();
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }
        private void SmoothHeightChanging()
        {
            if (Height == CController.Height && CController.Center.y == Height) return;
            if (!Mathf.Approximately(CController.Height, Height) || !Mathf.Approximately(CController.Center.y, Height / 2.0f))
            {
                CController.Height = Mathf.Lerp(CController.Height, Height, HeightChangingSmoothTime * Time.deltaTime);
                CController.Center = Vector3.Lerp(CController.Center, new Vector3(0, Height / 2.0f, 0), HeightChangingSmoothTime * Time.deltaTime);
            }
            else
            {
                CController.Height = Height;
                CController.Center = new Vector3(0, Height / 2.0f, 0);
            }
        }

        #region Rotation Methods

        /// <summary>
        /// Rotates character to face the desired direction.
        /// </summary>
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            desiredDirection.y = 0;
            if (desiredDirection.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            Rotate(targetRotation, turnSmoothTime);
        }

        /// <summary>
        /// Rotates character to face a position.
        /// </summary>
        public void RotateToPosition(Vector3 position, float turnSmoothTime = 0)
        {
            Vector3 desiredDirection = position - transform.position;
            RotateToDirection(desiredDirection.normalized, turnSmoothTime);
        }

        /// <summary>
        /// Rotates character to a target rotation.
        /// Delegates to controller wrapper for proper KinematicCharacterController integration.
        /// </summary>
        public void Rotate(Quaternion targetRotation, float turnSmoothTime = 0)
        {
            float smoothTime = turnSmoothTime > 0 ? turnSmoothTime : TurnSmoothTime;

            if (smoothTime > 0f)
            {
                // Smooth rotation using Slerp
                Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime * Time.fixedDeltaTime);
                CController.Rotate(smoothedRotation);
            }
            else
            {
                CController.Rotate(targetRotation);
            }
            CalculateDeltaYaw(targetRotation);
        }

        private void CalculateDeltaYaw(Quaternion targetRotation)
        {
            deltaYaw = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);
            if (Mathf.Abs(deltaYaw) < 0.01f)
            {
                deltaYaw = 0;
            }
        }

        #endregion

        #region Movement Methods

        public void Move(Vector3 moveDirection)
        {
            if (Input.MovementInput.magnitude > 0)
            {
                Velocity = Vector3.Lerp(Velocity, moveDirection.normalized * CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {
                if (Velocity.magnitude > 0.01f)
                {

                    Velocity = Vector3.Lerp(Velocity, Vector3.zero, Deceleration * Time.deltaTime);
                }
                else
                {
                    Velocity = Vector3.zero;
                }
            }
            CController.Move(Velocity);
        }
        public void Slide()
        {
            CController.Move( ProjectVelocity());
        }

        private Vector3 ProjectVelocity()
        {
            return Vector3.ProjectOnPlane(Velocity, sensors.BelowHit.normal) + Vector3.ProjectOnPlane(-transform.up * SpeedMultiplier, sensors.BelowHit.normal);
        }
        public void Jump()
        {
            if (Input.MovementInput.magnitude > 0)
            {
                Vector3 movement = new Vector3(Input.MovementInput.x, 0, Input.MovementInput.y);
                Vector3 cam = Camera.main.transform.forward;
                Velocity += Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement * SpeedMultiplier * JumpPlanarForce;
            }
            CController.Jump(JumpVerticalForce);
            OnJump.Invoke();
        }

        public void Roll()
        {
            Jump();
            Velocity += transform.forward * JumpPlanarForce;
            OnRoll.Invoke();
        }

        public void TurnBack()
        {
            OnBackTurn.Invoke();
        }
        #endregion


        public void SetInputReader(IMovementInputReader inputReader)
        {
            Input = inputReader;
        }
    }
}
