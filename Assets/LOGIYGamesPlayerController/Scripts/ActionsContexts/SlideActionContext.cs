using LOGIYGames;
using UnityEngine;
using UnityEngine.InputSystem;
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
        player.PlayerTimers.Add(slippingTimer);
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
        Input.CrouchEvent.AddListener(PerformRunToSlideJump);
    }

    private void OnDisable()
    {
        Input.CrouchEvent.RemoveListener(PerformRunToSlideJump);
    }

    private void PerformRunToSlideJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (player.TotalSpeedMultiplier > requiredSpeedMultiplierToSlip && player.IsGrounded && !slippingTimer.IsRunning && !IsSliding && sensors.GroundAngle > -30)
                {
                    IsSliding = true;
                    slippingTimer.Start();
                    player.HorizontalVelocity += MovementInput.magnitude * jumpSlidespeed * player.transform.forward;
                    player.InternalSpeedMultiplier = InternalSpeedMultiplier;
                    CrouchPressed = true;
                }
                break;
            case InputActionPhase.Canceled:
                CrouchPressed = false;
                break;
            default:
                break;
        }
    }

    protected override void GetMovementDirection()
    {
        Vector3 lookDirection = new Vector3(player.HorizontalVelocity.x, 0f, player.HorizontalVelocity.z);
        player.RotateToDirection(lookDirection, turnSmoothTime);
        moveDirection = new Vector3(sensors.BelowHit.normal.x, 0f, sensors.BelowHit.normal.z).normalized;
    }

    protected override void ChangeVelocity()
    {
        if (IsSliding)
        {

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(
            Vector3.down,
            sensors.BelowHit.normal
                );


            if (sensors.GroundAngle > SlideSlopeAngleLimit)
            {
                player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, InternalSpeedMultiplier, Time.deltaTime * player.Acceleration);
                player.HorizontalVelocity += projectedVelocity * Time.deltaTime * slideSpeed;

            }
            else
            {
                if (sensors.GroundAngle < -30)
                {
                    player.HorizontalVelocity = projectedVelocity * Time.deltaTime * slideSpeed;

                }
                else
                {
                    //To Do smoothly change InternalSpeed depend on angle
                    if (sensors.GroundAngle>0)
                    {
                        player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, 0, Time.deltaTime * player.Deceleration/2);

                        player.HorizontalVelocity = Vector3.Lerp(player.HorizontalVelocity, Vector3.zero, Time.deltaTime * player.Deceleration/2);
                    }
                    else if(sensors.GroundAngle < 0)
                    {
                        player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, 0, Time.deltaTime * player.Deceleration*2);
                       
                        player.HorizontalVelocity = Vector3.Lerp(player.HorizontalVelocity, Vector3.zero, Time.deltaTime * player.Deceleration*2);
                    }
                    else
                    {
                        player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, 0, Time.deltaTime * player.Deceleration);

                        player.HorizontalVelocity = Vector3.Lerp(player.HorizontalVelocity, Vector3.zero, Time.deltaTime * player.Deceleration);
                    }
                        player.HorizontalVelocity += projectedVelocity * Time.deltaTime * slideSpeed;
                }

            }

        }
    }



    private void Update()
    {
       // if (!IsOwner) return;
        if (!player.IsGrounded)
        {
            IsSliding = false;
            return;
        }
        if (slippingTimer.IsRunning)
        {
            return;
        }
        if (IsSliding && player.HorizontalVelocity.magnitude > speedTresholdForExitSliding && CrouchPressed)
        {
            return;
        }

        if (Mathf.Abs(sensors.GroundAngle) > SlideSlopeAngleLimit)
        {
            IsSliding = true;
        }
        else
        {
            IsSliding = false;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        animator.SetBool(isSlidingHash, false);
    }

    protected override void UpdateAnimations()
    {
        animator.SetBool(isSlidingHash, IsSliding);
    }



}