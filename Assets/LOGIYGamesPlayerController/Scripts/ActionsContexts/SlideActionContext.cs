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

    [SerializeField] private float maxSlideAngle = 85f;
    private Vector3 slideDirection;

    [Header("Slip Settings")]
    [SerializeField] private float jumpSlidespeed = 2f;
    CountdownTimer slippingTimer;
    [SerializeField] private float slipTime = 1f;
    private float SlideSlopeAngleLimit => Mathf.Atan(FrictionCoefficient) * Mathf.Rad2Deg;
    [SerializeField] private float requiredSpeedMultiplierToSlip = 0.5f;
    int isSlidingHash = Animator.StringToHash("IsSliding");
    [Header("Debug")]
    float currentSlopeAngle;

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
                if (player.TotalSpeedMultiplier > requiredSpeedMultiplierToSlip && player.IsGrounded && !slippingTimer.IsRunning && !IsSliding)
                {
                    IsSliding = true;
                    slippingTimer.Start();
                    player.HorizontalVelocity += MovementInput.magnitude * jumpSlidespeed * player.transform.forward;
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



    private void ApplyGravity()
    {
        player.VerticalVelocity = -20f;
    }

    protected override Vector3 RotateAndGetMovementDirection()
    {
        Vector3 lookDirection = new Vector3(player.HorizontalVelocity.x, 0f, player.HorizontalVelocity.z);
        player.RotateToDirection(lookDirection, turnSmoothTime);
        return new Vector3(sensors.BelowHit.normal.x, 0f, sensors.BelowHit.normal.z).normalized;
    }

    protected override void ChangeVelocity(Vector3 moveDirection)
    {
        if (IsSliding)
        {
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(
            Vector3.down,
            sensors.BelowHit.normal
                );

            player.HorizontalVelocity += projectedVelocity * Time.deltaTime * player.Acceleration;
        }
    }



    private void Update()
    {
        if (!IsOwner) return;
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

        if (sensors.GroundAngle > SlideSlopeAngleLimit)
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