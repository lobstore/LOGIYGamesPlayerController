using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(CharacterModule))]
[RequireComponent(typeof(SensorsModule))]
[RequireComponent(typeof(CharacterController))]
[DefaultExecutionOrder(-1)]
public class SlideActionContext : NetworkBehaviour, IActionContext
{
    [Header("Component References")]
    [SerializeField] private Animator animator;
    private CharacterModule player;
    private SensorsModule sensors;
    private PlayerCameraManager cameraManager;
    private CharacterController characterController;
    private PlayerInputsManager input;

    [Header("Slide Settings")]
    [SerializeField] private float slideHeightMultiplier = 0.2f;
    [SerializeField] private float speedTresholdForExitSliding = 0.5f;
    private float SlideHeight;
    private float StandingHeight;
    [SerializeField] private float turnSmoothTime = 20f;
    [SerializeField] private float InternalSpeedMultiplier = 1;
    [field: SerializeField] public float Acceleration { get; set; } = 10f;
    [field: SerializeField] public float Deceleration { get; set; } = 10f;
    [field: SerializeField] public MotionType MotionType { get; private set; }
    [Header("Slope Settings")]

    [SerializeField] private float maxSlideAngle = 85f;
    private Vector3 slideDirection;

    [Header("Slip Settings")]
    [SerializeField] private float jumpSlidespeed = 2f;
    CountdownTimer slippingTimer;
    [SerializeField] private float slipTime = 1f;
    private float toSlideSlopeLimit;
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
    private void Awake()
    {
        InitializeComponents();
        slippingTimer = new CountdownTimer(slipTime);
        player.PlayerTimers.Add(slippingTimer);
    }

    private void InitializeComponents()
    {
        player = GetComponent<CharacterModule>();
        sensors = GetComponent<SensorsModule>();
        cameraManager = GetComponent<PlayerCameraManager>();
        characterController = GetComponent<CharacterController>();
        StandingHeight = characterController.height;
        SlideHeight = StandingHeight * slideHeightMultiplier;
    }

    private void OnEnable()
    {
        slippingTimer.Reset(slipTime);
        input = PlayerInputsManager.Instance;
        input.Crouched.AddListener(RunToSlide);
    }
    private void OnDisable()
    {
        input.Crouched.RemoveListener(RunToSlide);
    }
    public void OnFixedUpdate()
    {
        if (!IsOwner) return;
        Slide();
    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
    }
    private void RunToSlide()
    {
        if (!IsOwner) return;
        if (player.TotalSpeedMultiplier > requiredSpeedMultiplierToSlip && player.IsGrounded && !slippingTimer.IsRunning && !IsSliding)
        {
            IsSliding = true;
            slippingTimer.Start();
            player.HorizontalVelocity += input.MovementInput.magnitude * jumpSlidespeed * player.transform.forward;

        }
    }
    private void CalculateSlopeLimit()
    {
        toSlideSlopeLimit = Mathf.Atan(FrictionCoefficient) * Mathf.Rad2Deg;
    }
    private void Slide()
    {
        CalculateSlideDirection();
        RotatePlayer();
        ApplyGravity();

    }
    private void ApplyGravity()
    {
        player.VerticalVelocity = -20f;
    }
    private void CalculateSlideDirection()
    {
        slideDirection = new Vector3(sensors.BelowHit.normal.x, 0f, sensors.BelowHit.normal.z).normalized;
    }

    private void UpdatePlayerVelocity()
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

    private void RotatePlayer()
    {
        Vector3 lookDirection = new Vector3(player.HorizontalVelocity.x, 0f, player.HorizontalVelocity.z);
        player.RotateToDirection(lookDirection, turnSmoothTime);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
    }
    private void Update()
    {
        if (!IsOwner) return;
        CalculateSlopeLimit();
        UpdatePlayerVelocity();
        if (!player.IsGrounded)
        {
            IsSliding = false;
            return;
        }
        if (slippingTimer.IsRunning)
        {
            return;
        }
        if (IsSliding && player.HorizontalVelocity.magnitude > speedTresholdForExitSliding && input.IsCrouching)
        {
            return;
        }

        if (sensors.GroundAngle > toSlideSlopeLimit)
        {
            IsSliding = true;
        }
        else
        {
            IsSliding = false;
        }
    }
    private void LateUpdate()
    {

        UpdateAnimation();

    }
    public void EnterState()
    {
        if (MotionType == MotionType.AnimatorController)
        {
            animator.applyRootMotion = true;
        }
        else
        {
            animator.applyRootMotion = false;
        }
        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;
        player.InternalSpeedMultiplier = InternalSpeedMultiplier;
        UpdateAnimation();
    }

    public void ExitState()
    {
        animator.SetBool(isSlidingHash, false);
    }

    private void UpdateAnimation()
    {
        animator.SetBool(isSlidingHash, IsSliding);
    }



}