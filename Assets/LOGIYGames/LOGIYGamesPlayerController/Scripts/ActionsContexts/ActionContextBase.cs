using LOGIYGames.CharacterCore;
using UnityEngine;
namespace LOGIYGames
{
    public abstract class ActionContextBase : MonoBehaviour
    {
        int isGroundedHash = Animator.StringToHash("IsGrounded");
        [Header("Velocity Data")]
        [SerializeField] protected float Acceleration = 0f;
        [SerializeField] protected float Deceleration = 0f;
        [SerializeField] protected float TurnSmoothTime = 10f;
        [Tooltip("Should the character move after the turn, or turn after the movement (if false, then the direction of movement always coincides with transform.forward)")]
        [SerializeField] protected bool moveBeforeRotation;
        protected float InternalSpeedMultiplier = 0f;
        protected const float angleThreshold = 45f;
        [Header("References")]
        protected Character Character;
        protected SensorsModule Sensors;
        protected Animator animator;
        protected CharacterController CController;
        protected CharacterGravityModule CharacterGravity;
        protected bool isMoving;
        protected float deltaYaw;
        protected Vector3 moveDirection;
        public Vector2 MovementInput => Character.MovementInput;

        private float lastYRotation;
        [Tooltip("Should the motion vector move relative to the object, or relative to the object and the surface normal")]
        [SerializeField] protected bool UseProjectionOnPlane = true;
        [Tooltip("This is how an object should be set in motion")]
        [SerializeField] protected MotionType MotionType;

        protected virtual void Awake()
        {
            Sensors = GetComponent<SensorsModule>();
            Character = GetComponent<Character>();
            animator = GetComponent<Animator>();
            CController = GetComponent<CharacterController>();
            CharacterGravity = GetComponent<CharacterGravityModule>();
        }

        public virtual void EnterState()
        {
            // if (!IsOwner) return;

            InitializeMotionSystem();

            Character.Acceleration = Acceleration;
            Character.Deceleration = Deceleration;

            UpdateAnimations();
        }

        protected virtual void InitializeMotionSystem()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    animator.applyRootMotion = false;
                    break;
                case MotionType.AnimatorController:
                    animator.applyRootMotion = true;
                    break;
                default:
                    break;
            }
        }

        public virtual void ExitState()
        {
            // if (!IsOwner) return;
            UpdateAnimations();
        }

        public virtual void OnUpdate()
        {
            //  if (!IsOwner) return;
        }

        public virtual void OnFixedUpdate()
        {
            //  if (!IsOwner) return;
            Move();

            UpdateAnimations();
        }

        protected virtual void Move()
        {
            GetDeltaAngle();
            GetMovementDirection();
            DebugDraw.DrawVector(transform.position, Character.HorizontalVelocity, 1, 1, Color.blue, 0);
            ChangeVelocity();
            ApplyMovement();
            Rotate();
        }
        protected virtual void ApplyMovement()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    Vector3 newVelocity = HandleSteepWalls(Character.HorizontalVelocity);
                    CController.Move(newVelocity * Time.deltaTime);
                    break;
                case MotionType.AnimatorController:
                    break;
                default:
                    break;
            }

        }
        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = Sensors.BelowHit.normal;
            bool validAngle = Sensors.IsValidSlope(normal);

            if (!validAngle && CharacterGravity.VerticalVelocity < 0f)
                velocity = Vector3.ProjectOnPlane(velocity, normal);

            return velocity;
        }
        protected virtual void ChangeVelocity()
        {
            if (MovementInput.magnitude > 0)
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, moveDirection * Character.CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Character.Deceleration * Time.deltaTime);
            }

        }


        protected virtual void GetMovementDirection()
        {
            switch (CameraManager.Instance.CameraFocusingState)
            {
                case CameraFocusingState.FreeLook:
                    moveDirection = GetMovementDirectionRelativeCamera();
                    break;
                case CameraFocusingState.LookForward:
                    moveDirection = GetMovementDirectionAlongCamera();
                    break;
                case CameraFocusingState.Focus:
                    moveDirection = GetMovementDirectionAlongCamera();
                    break;
                default:
                    break;
            }
            if (UseProjectionOnPlane)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, Sensors.BelowHit.normal).normalized;
            }
        }


        protected virtual Vector3 GetMovementDirectionAlongCamera()
        {
            var fwd = Camera.main.transform.forward;
            fwd.y = 0;
            var rght = Camera.main.transform.right;
            rght.y = 0;
            return rght.normalized * MovementInput.x + fwd.normalized * MovementInput.y;
        }

        protected virtual Vector3 GetMovementDirectionRelativeCamera()
        {
            if (moveBeforeRotation)
            {
                Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);
                Vector3 cam = Camera.main.transform.forward;
                print(Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement);
                return Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement;
            }
            else
            {
                return Character.transform.forward;
            }
        }
        protected virtual void Rotate()
        {
            if (!Character.IsUnderPlayerControl) return;
            switch (CameraManager.Instance.CameraFocusingState)
            {
                case CameraFocusingState.FreeLook:
                    RotateRelativeCamera();
                    break;
                case CameraFocusingState.LookForward:
                    RotateAlongCamera();
                    break;
                case CameraFocusingState.Focus:
                    RotateToTarget();
                    break;
                default:
                    break;
            }
        }

        private void RotateToTarget()
        {
            if (Character.Target == null)
            {
                return;
            }

            Character.RotateToPosition(Character.Target.position);
        }

        private void RotateRelativeCamera()
        {
            if (MovementInput.magnitude > 0f)
            {
                // Рассчитываем угол поворота по направлению движения
                var targetAngle = Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                Character.Rotate(targetRotation, TurnSmoothTime);
            }
        }

        private void RotateAlongCamera()
        {
            var targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            Character.Rotate(targetRotation, TurnSmoothTime);
        }

        protected virtual void UpdateAnimations()
        {
            animator.SetBool(isGroundedHash, Sensors.IsGrounded);
        }

        protected virtual void GetDeltaAngle()
        {
            float currentYRotation = transform.eulerAngles.y;
            deltaYaw = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;
            lastYRotation = currentYRotation;
        }
    }
}