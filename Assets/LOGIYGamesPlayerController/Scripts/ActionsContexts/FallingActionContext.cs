using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(CharacterModule))]
public class FallingActionContext : NetworkBehaviour, IActionContext
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    int landingStateHash = Animator.StringToHash("LandingState");
    int isFallingHash = Animator.StringToHash("IsFalling");
    [Header("Timing Settings")]
    [SerializeField] private float turnSmoothTime = 10f;
    [SerializeField] private float landingDuration = 0.1f;
    [SerializeField] private float minFallingTimeToLandingTransition = 0.8f;
    [SerializeField] private float fallingTimeForHardLanding = 1f;
    [SerializeField] private float hardLandingDuration = 1.5f;
    [SerializeField] private bool autoCalculateLandingDuration = false;

    [Header("Movement Settings")]
    [SerializeField] private float landingSpeedMultiplier = 0f;
    [SerializeField] private float fallingMoveSpeedMultiplier = 0.5f;
    [field: SerializeField] public float Acceleration { get; private set; } = 5f;
    [field: SerializeField] public float Deceleration { get; private set; } = 3f;
    [field: SerializeField] public MotionType MotionType { get; private set; }

    [Header("Component References")]
    private CharacterModule player;
    private PlayerCameraManager cameraManager;
    private PlayerMovementInputManager input;

    // State Management
    private CountdownTimer landingCoolDownTimer;
    private StopwatchTimer fallingTimer;
    private float turnSmoothVelocity;
    public bool IsLanding { get; private set; }
    public float FallingTime => fallingTimer.GetTime();
    public float InternalSpeedMultiplier { get; set; }

    private void Awake()
    {
        InitializeComponents();
        InitializeTimers();
    }

    private void InitializeComponents()
    {
        player = GetComponent<CharacterModule>();
        cameraManager = GetComponent<PlayerCameraManager>();
    }

    private void InitializeTimers()
    {
        landingCoolDownTimer = new CountdownTimer(landingDuration);
        fallingTimer = new StopwatchTimer();
        player.PlayerTimers.Add(landingCoolDownTimer);
        player.PlayerTimers.Add(fallingTimer);
    }

    private void OnEnable() => input = PlayerMovementInputManager.Instance;

    public void StartFallingTimer() => fallingTimer.Start();

    public void StopFallingTimer()
    {
        fallingTimer.Stop();

        if (!autoCalculateLandingDuration) return;

        CalculateLandingParameters();
        landingCoolDownTimer.Reset(landingDuration);
    }

    private void CalculateLandingParameters()
    {
        if (FallingTime > minFallingTimeToLandingTransition)
        {
            if (FallingTime > fallingTimeForHardLanding)
            {
                landingDuration = hardLandingDuration;
                landingSpeedMultiplier = 0f;
            }
            else
            {
                landingDuration = Mathf.Log10(FallingTime + 1);
                landingSpeedMultiplier = 0.1f;
            }
        }
        else
        {
            landingDuration = 0f;
            landingSpeedMultiplier = 1f;
        }
    }

    public void OnLanding()
    {
        landingCoolDownTimer.Start();
        IsLanding = true;

        SetLandingAnimationState();
    }

    private void SetLandingAnimationState()
    {
        animator.SetInteger(landingStateHash, FallingTime <= fallingTimeForHardLanding
            ? 1 : 2);
    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
        Move();
    }
    private void Move()
    {


        if (cameraManager.IsFP)
        {
            MoveAlongCamera();
        }
        else if (input.MovementInput.magnitude > 0)
        {
            MoveRelativeCamera();
        }
    }

    private void MoveAlongCamera()
    {
        float targetAngle = cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(
            player.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            1 / (turnSmoothTime * 4));

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDir = player.transform.right * input.MovementInput.x
                        + player.transform.forward * input.MovementInput.y;

        UpdatePlayerVelocity(moveDir);
    }

    private void MoveRelativeCamera()
    {
        float targetAngle = Mathf.Atan2(input.MovementInput.x, input.MovementInput.y) * Mathf.Rad2Deg
                          + cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(
            player.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            1 / turnSmoothTime);

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        UpdatePlayerVelocity(moveDir);
    }

    private void UpdatePlayerVelocity(Vector3 moveDirection)
    {
        Vector3 desiredVelocity = moveDirection * player.CurrentSpeed;
        player.HorizontalVelocity = Vector3.Lerp(
            player.HorizontalVelocity,
            desiredVelocity,
            Time.deltaTime * Acceleration);
    }

    public void SpeedControl()
    {
        if (!IsOwner) return;

        if (IsLanding || input.MovementInput.magnitude == 0)
        {
            player.InternalSpeedMultiplier = 0f;
            return;
        }

        player.InternalSpeedMultiplier = Mathf.Lerp(
            player.InternalSpeedMultiplier,
            fallingMoveSpeedMultiplier,
            Time.deltaTime * Acceleration);
    }

    public void EnterState()
    {
        animator.applyRootMotion = false;
        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;
        StartFallingTimer();
        animator?.SetBool(isFallingHash, true);
        animator?.SetInteger(landingStateHash, 0);
    }

    public void ExitState()
    {
        animator.applyRootMotion = true;
        animator?.SetBool(isFallingHash, false);
        animator?.SetInteger(landingStateHash, 0);
        StopFallingTimer();

    }




    private void PlayImmediateAnimation(string name) =>
        animator?.Play(name);

    private void Update()
    {
        if (!IsOwner) return;

        if (landingCoolDownTimer.IsFinished)
        {
            IsLanding = false;

        }
    }
}