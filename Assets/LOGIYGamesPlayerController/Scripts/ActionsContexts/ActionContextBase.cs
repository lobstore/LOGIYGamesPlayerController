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
        [Tooltip("should the character move after the turn, or turn after the movement (if false, then the direction of movement always coincides with transform.forward)")]
        [SerializeField] protected bool moveBeforeRotation;
        protected float InternalSpeedMultiplier = 0f;
        protected const float angleThreshold = 45f;
        [Header("References")]
        protected Character Character;
        protected SensorsModule Sensors;
        protected Animator animator;

        protected bool isMoving;
        protected float deltaYaw;
        protected Vector3 moveDirection;
        public Vector2 MovementInput => Character.MovementInput;

        private float lastYRotation;

        [SerializeField] protected bool UseProjectionOnPlane = true;
        [SerializeField] protected MotionType MotionType;

        protected virtual void Awake()
        {

            Sensors = GetComponent<SensorsModule>();
            Character = GetComponent<Character>();
            animator = GetComponent<Animator>();

        }

        public virtual void EnterState()
        {
            // if (!IsOwner) return;

            ApplyRootMotion();

            Character.Acceleration = Acceleration;
            Character.Deceleration = Deceleration;

            UpdateAnimations();
        }

        protected virtual void ApplyRootMotion()
        {
            if (MotionType == MotionType.AnimatorController)
            {
                animator.applyRootMotion = true;
            }
            else
            {
                animator.applyRootMotion = false;
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
            Rotate();
        }

        protected virtual void ChangeVelocity()
        {

            if (MovementInput.magnitude > 0)
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, moveDirection * Character.CurrentSpeed, Acceleration * Time.fixedDeltaTime);

            }
            else
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Character.Deceleration * Time.fixedDeltaTime);
            }

        }


        protected virtual void GetMovementDirection()
        {
            if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
            {
                moveDirection = GetMovementDirectionAlongCamera();
            }
            else
            {
                moveDirection = GetMovementDirectionRelativeCamera();
            }
            if (UseProjectionOnPlane)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, Sensors.BelowHit.normal).normalized;
            }
        }


        protected virtual Vector3 GetMovementDirectionAlongCamera()
        {
            return Character.transform.right * MovementInput.x + Character.transform.forward * MovementInput.y;
        }

        protected virtual Vector3 GetMovementDirectionRelativeCamera()
        {
            if (moveBeforeRotation)
            {
                Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);
                Vector3 cam = Camera.main.transform.forward;
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
            if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
            {
                RotateAlongCamera();
            }
            else
            {
                RotateRelativeCamera();
            }
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