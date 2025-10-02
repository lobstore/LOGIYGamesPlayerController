using LOGIYGames;
using UnityEngine;
using UnityEngine.InputSystem;
using LOGIYGames.Timers;
namespace LOGIYGames
{
    [DefaultExecutionOrder(-1)]
    public class SlideActionContext : GroundedActionContext
    {
        [Header("Component References")]
        private CharacterController characterController;

        [Header("Slide Settings")]
        [SerializeField] private float slideHeightMultiplier = 0.2f;
        [SerializeField] private float speedTresholdForExitSliding = 0.5f;
        private float SlideHeight;
        private float StandingHeight;
        [SerializeField] private float turnSmoothTime = 20f;
        [Header("Slope Settings")]

        [Header("Slip Settings")]
        [SerializeField] private float jumpSlidespeed = 2f;
        CountdownTimer slippingTimer;
        [SerializeField] private float slipTime = 1f;
        [SerializeField] private float slideSpeed = 1f;
        private float SlideSlopeAngleLimit => Mathf.Atan(FrictionCoefficient) * Mathf.Rad2Deg;
        [SerializeField] private float requiredSpeedMultiplierToSlip = 0.5f;
        int isSlidingHash = Animator.StringToHash("IsSliding");
        [SerializeField] private bool CanSlide;

        public float FrictionCoefficient
        {
            get
            {
                return characterController.sharedMaterial.dynamicFriction;
            }
        }

        public bool IsSliding { get; private set; }
        public bool CrouchPressed { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
            slippingTimer = new CountdownTimer(slipTime);
        }

        private void InitializeComponents()
        {
            characterController = GetComponent<CharacterController>();
            StandingHeight = characterController.height;
            SlideHeight = StandingHeight * slideHeightMultiplier;
        }

        private void OnEnable()
        {
            slippingTimer.Reset(slipTime);
        }
        private void FixedUpdate()
        {
            if (Character.CrouchPressed)
            {
                if (Character.TotalSpeedMultiplier > requiredSpeedMultiplierToSlip && Sensors.IsGrounded && !slippingTimer.IsRunning && !IsSliding && Sensors.GroundAngle > -30)
                {
                    IsSliding = true;
                    CrouchPressed = true;
                    slippingTimer.Start();
                }
            }
            else
            {
                CrouchPressed = false;
            }
        }

        protected override void GetMovementDirection()
        {
            Vector3 lookDirection = new Vector3(Character.HorizontalVelocity.x, 0f, Character.HorizontalVelocity.z);
            Character.RotateToDirection(lookDirection, turnSmoothTime);
            moveDirection = new Vector3(Sensors.BelowHit.normal.x, 0f, Sensors.BelowHit.normal.z).normalized;
        }

        protected override void ChangeVelocity()
        {
            if (IsSliding)
            {

                Vector3 projectedVelocity = Vector3.ProjectOnPlane(
                Vector3.down,
                Sensors.BelowHit.normal
                    );


                if (Sensors.GroundAngle > SlideSlopeAngleLimit)
                {
                    Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier, Time.deltaTime * Character.Acceleration);
                    Character.HorizontalVelocity += projectedVelocity * Time.deltaTime * Character.CurrentSpeed;

                }
                else
                {
                    if (Sensors.GroundAngle < -30)
                    {
                        Character.HorizontalVelocity = projectedVelocity * Time.deltaTime * Character.CurrentSpeed;

                    }
                    else
                    {
                        //To Do smoothly change InternalSpeed depend on angle
                        if (Sensors.GroundAngle > 0)
                        {
                            Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Time.deltaTime * Character.Deceleration / 2);

                            Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Time.deltaTime * Character.Deceleration / 2);
                        }
                        else if (Sensors.GroundAngle < 0)
                        {
                            Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Time.deltaTime * Character.Deceleration * 2);

                            Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Time.deltaTime * Character.Deceleration * 2);
                        }
                        else
                        {
                            Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Time.deltaTime * Character.Deceleration);

                            Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Time.deltaTime * Character.Deceleration);
                        }
                        Character.HorizontalVelocity += projectedVelocity * Time.deltaTime * Character.CurrentSpeed;
                    }

                }

            }
        }



        private void Update()
        {
            // if (!IsOwner) return;
            if (!Sensors.IsGrounded)
            {
                IsSliding = false;
                return;
            }
            if (slippingTimer.IsRunning)
            {
                return;
            }
            if (IsSliding && Character.HorizontalVelocity.magnitude > speedTresholdForExitSliding && CrouchPressed)
            {
                return;
            }

            if (Mathf.Abs(Sensors.GroundAngle) > SlideSlopeAngleLimit && CanSlide)
            {
                IsSliding = true;
            }
            else
            {
                IsSliding = false;
            }
        }
        public override void EnterState()
        {
            base.EnterState();

            Character.HorizontalVelocity += MovementInput.magnitude * jumpSlidespeed * Character.transform.forward;
            Character.InternalSpeedMultiplier = InternalSpeedMultiplier;
        }
        public override void ExitState()
        {
            base.ExitState();
            animator.SetBool(isSlidingHash, false);

        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();
            animator.SetBool(isSlidingHash, IsSliding);
        }



    }
}