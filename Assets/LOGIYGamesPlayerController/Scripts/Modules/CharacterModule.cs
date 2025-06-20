using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    [RequireComponent(typeof(SensorsModule))]
    [RequireComponent(typeof(CharacterController))]
    [DefaultExecutionOrder(-2)]
    public class CharacterModule : NetworkModuleBase
    {

        [Header("References")]
        private Animator Animator;
        private SensorsModule Sensors = null;
        private CharacterController controller = null;
        public List<Timer> PlayerTimers { get; private set; } = new List<Timer>();

        int isGroundedHash = Animator.StringToHash("IsGrounded");
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
        public float CurrentSpeed => TotalSpeedMultiplier * BaseSpeed;
        public Vector3 Velocity => new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        public Vector3 HorizontalVelocity { get => horizontalVelocity; set => horizontalVelocity = value; }
        public float VerticalVelocity { get => verticalVelocity; set => verticalVelocity = value; }
        private Vector3 horizontalVelocity;
        private float verticalVelocity;
        #endregion

        [field: SerializeField] public float BaseSpeed { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public CountdownTimer ExitingWallTimer { get; set; }

        [Space(2)]
        [Header("Physics")]
        [SerializeField] bool useGravity;
        [SerializeField] bool useInertia;
        [SerializeField] float gravityMultiplier;
        public bool UseGravity { get => useGravity; set => useGravity = value; }

        public bool IsGrounded
        {
            get => Sensors.IsObstacleBelow && IsValidSlope(Sensors.BelowHit.normal);
        }

        public float HeightChangingSmoothTime { get; private set; } = 4f;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            Sensors = GetComponent<SensorsModule>();
            Height = controller.height;
            Weight = 1f;
            ExitingWallTimer = new CountdownTimer(0.05f);
            PlayerTimers.Add(ExitingWallTimer);
            Animator = GetComponent<Animator>();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            HandleTimers();
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            ApplyForces();
            ApplyGravity();
        }

        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            SmoothHeightChanging();
            Animator.SetBool(isGroundedHash, IsGrounded);
        }
        void HandleTimers()
        {
            foreach (var timer in PlayerTimers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        private void SmoothHeightChanging()
        {
            if (controller.height == Height) return;
            if (controller.height > Height + 0.01f || controller.height < Height - 0.01f)
            {
                controller.height = Mathf.Lerp(controller.height, Height, HeightChangingSmoothTime * Time.deltaTime);
            }
            else
            {
                controller.height = Height;
            }

        }

        /// <summary>
        /// Rotate object to specified direction with specified speed
        /// </summary>
        /// <param name="desiredDirection">Direction to rotate</param>
        /// <param name="turnSmoothTime">Turn speed, instant turn if 0, if !=0, method should be used in update</param>
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            Rotate(targetRotation, turnSmoothTime);
        }
        public void RotateToPosition(Vector3 position)
        {
            Vector3 desiredDirection = position - transform.position;
            RotateToDirection(desiredDirection.normalized);
        }
        public void Rotate(Quaternion targetRotation, float turnSmoothTime)
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

        private void ApplyForces()
        {
            Vector3 newVelocity;
            if (useInertia)
            {
                if (horizontalVelocity.magnitude > 0.01f)
                {
                    horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Deceleration * Time.deltaTime);
                }
                else
                {
                    horizontalVelocity = Vector3.zero;
                }
            }

            newVelocity = HandleSteepWalls(new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z));
            controller.Move(newVelocity * Time.deltaTime);
        }
        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = Sensors.BelowHit.normal;
            bool validAngle = IsValidSlope(normal);

            if (!validAngle && verticalVelocity < 0f)
                velocity = Vector3.ProjectOnPlane(velocity, normal);

            return velocity;
        }

        private static bool IsValidSlope(Vector3 normal)
        {
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= 85;
            return validAngle;
        }

        private void ApplyGravity()
        {
            if (!useGravity) { return; }
            if (IsGrounded
                && verticalVelocity < 0)
            {
                verticalVelocity = -4 / (gravityMultiplier+0.001f);
            }
            else
            {
                verticalVelocity += (Physics.gravity.y - Weight/10)  * Time.deltaTime * gravityMultiplier;
            }
        }
        public void ResetMotion()
        {
            horizontalVelocity = Vector3.zero;
            verticalVelocity = 0;
            Acceleration = 0;
            Deceleration = 0;
            InternalSpeedMultiplier = 0;
        }
    }
}