using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(CharacterModule))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SensorsModule))]
public class CrouchActionContext : NetworkBehaviour, IActionContext
{
    [Header("Animation Parameters")]
    [SerializeField] private Animator animator;
    private int isMovingHash;
    private int speedHash;
    private int verticalSpeedHash;
    private int horizontalSpeedHash;

    [Header("Movement Settings")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float crouchHeightMultiplier = 0.5f;
    [field: SerializeField] public float Acceleration { get; private set; } = 5f;
    [field: SerializeField] public float Deceleration { get; private set; } = 25f;
    private float turnSmoothTime = 20;
    private float turnSmoothVelocity;
    [field: SerializeField] public MotionType MotionType { get; private set; }
    [Header("Component References")]
    private SensorsModule sensors;
    private CharacterModule player;
    private CharacterController characterController;
    private PlayerInputsManager input;
    private PlayerCameraManager cameraManager;
    private float smoothTime = 0.5f;

    // Properties
    public Vector2 MovementInput => input.MovementInput;
    public bool IsSprinting => input.IsShifting;
    public bool IsCrouching { get; private set; }
    public float CrouchHeight { get; private set; }
    public float StandingHeight { get; private set; }

    private void Awake()
    {
        InitializeComponents();
        InitializeAnimationHashes();
        InitializeHeightValues();
    }

    private void InitializeComponents()
    {
        player = GetComponent<CharacterModule>();
        cameraManager = GetComponent<PlayerCameraManager>();
        characterController = GetComponent<CharacterController>();
        sensors = GetComponent<SensorsModule>();
    }

    private void InitializeAnimationHashes()
    {
        isMovingHash = Animator.StringToHash("IsMoving");
        speedHash = Animator.StringToHash("Speed");
        verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    }

    private void InitializeHeightValues()
    {
        StandingHeight = characterController.height;
        CrouchHeight = StandingHeight * crouchHeightMultiplier;
    }

    private void OnEnable() => input = PlayerInputsManager.Instance;
    public void OnFixedUpdate()
    {
        if (!IsOwner) return; 
        CrouchMove();
    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
        SpeedControl();
    }
    private void CrouchMove()
    {

        if (cameraManager.IsFP)
        {
            MoveAlongCamera();
        }
        else
        {
            MoveRelativeCamera();
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        animator.SetFloat(speedHash, player.TotalSpeedMultiplier, 0.05f, Time.deltaTime);
        if (cameraManager.IsFP)
        {
            animator.SetFloat(verticalSpeedHash, MovementInput.y * player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, MovementInput.x * Mathf.Clamp01(player.TotalSpeedMultiplier), smoothTime, Time.deltaTime);
        }
        else
        {
            animator.SetFloat(verticalSpeedHash, player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, 0);
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
        Vector3 moveDir = player.transform.right * input.MovementInput.x + player.transform.forward * input.MovementInput.y;
        player.HorizontalVelocity = moveDir * player.CurrentSpeed;
    }

    private void MoveRelativeCamera()
    {
        if (input.MovementInput.magnitude == 0) return;

        float targetAngle = Mathf.Atan2(input.MovementInput.x, input.MovementInput.y) * Mathf.Rad2Deg
                          + cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(
            player.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            1 / turnSmoothTime);

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        player.HorizontalVelocity = moveDir * player.CurrentSpeed;
    }

    private void CheckForCrouching()
    {
        IsCrouching = sensors.IsObstacleAbove || input.IsCrouching;
    }

    private void SpeedControl()
    {
        if (!IsOwner) return;

        bool isMoving = input.MovementInput.magnitude > 0;
        animator.SetBool(isMovingHash, isMoving);

        player.InternalSpeedMultiplier = isMoving
            ? Mathf.Lerp(player.InternalSpeedMultiplier, crouchSpeedMultiplier, Time.deltaTime * Acceleration)
            : 0f;
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
        player.Height = CrouchHeight;
    }

    public void ExitState() => player.Height = StandingHeight;

    private void Update()
    {
        if (!IsOwner) return;
        CheckForCrouching();
    }
}