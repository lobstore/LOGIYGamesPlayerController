using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(CharacterModule))]
public class LocomotionActionContext : NetworkBehaviour, IActionContext
{
    [Header("Animation Parameters")]
    private Animator animator;
    private int isMovingHash = Animator.StringToHash("IsMoving");
    private int yawInputHash = Animator.StringToHash("Yaw Input");
    private int speedHash = Animator.StringToHash("Speed");
    private int isSprintingHash = Animator.StringToHash("IsSprinting");
    private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    [SerializeField] float smoothTime = 0.3f;
    private float lastYRotation;
    private float turnThreshold = 1f; // чувствительность поворота (в градусах)

    [Header("Movement Settings")]

    [SerializeField] private float sprintTurnSmoothTime = 8f;
    [SerializeField] private float normalTurnSmoothTime = 10f;
    [field: SerializeField] public float Acceleration { get; private set; } = 2f;
    [field: SerializeField] public float Deceleration { get; private set; } = 25f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float runSpeedMultiplier = 1f;
    [field: SerializeField] public MotionType MotionType { get; private set; }
    public bool CanSprint { get; set; } = true;
    [SerializeField]
    private bool useAutoCalculatedPlayerSpeedMultiplier = false;
    [Tooltip("Used if UseAutoCalculatedPlayerSpeedMultiplier is On")]
    [Range(0, 1)]
    [SerializeField]
    private float slopeAffectRate;
    [Header("Component References")]
    private CharacterModule player;
    private PlayerCameraManager cameraManager;
    private PlayerInputsManager input;
    private SensorsModule sensors;
    // Movement State
    private float turnSmoothVelocity;
    private float currentTurnSmoothTime;
    public bool IsSprinting { get; private set; } = true;
    public Vector2 MovementInput => input != null ? input.MovementInput : Vector2.zero;

    bool isMoving;
    private float deltaY;

    private void Awake()
    {
        InitializeComponents();
        currentTurnSmoothTime = normalTurnSmoothTime;
    }

    private void InitializeComponents()
    {
        player = GetComponent<CharacterModule>();
        cameraManager = GetComponent<PlayerCameraManager>();
        animator = GetComponent<Animator>();
        sensors = GetComponent<SensorsModule>();
    }

    private void OnEnable()
    {
        input = PlayerInputsManager.Instance;
        if (input == null)
        {
            Debug.LogWarning("PlayerMovementInputManager.Instance was not found");
        }
    }
    public void OnFixedUpdate()
    {
        if (!IsOwner) return;
        DeltaAngle();
        Move();
        UpdateAnimation();
    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
        SpeedControl();

    }
    private void Move()
    {

        if (cameraManager.IsFP)
        {
            MoveAlongCamera();
        }
        else
        {
            MoveRelativeCamera();
        }

    }

    private void UpdateAnimation()
    {
        isMoving = MovementInput.magnitude > 0;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetFloat(speedHash, player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
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
        animator.SetFloat(yawInputHash, Mathf.Clamp(deltaY, -1, 1), smoothTime, Time.deltaTime);
        animator.SetBool(isSprintingHash, IsSprinting);
    }

    float Normalize(float input, float min, float max)
    {
        float average = (min + max) / 2;
        float range = (max - min) / 2;
        float normalized_x = (input - average) / range;
        return normalized_x;
    }
    private void MoveAlongCamera()
    {
        float targetAngle = cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(
            player.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            1 / (currentTurnSmoothTime * 4));

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDir = player.transform.right * MovementInput.x + player.transform.forward * MovementInput.y;
        moveDir = Vector3.ProjectOnPlane(moveDir, sensors.BelowHit.normal).normalized;
        if (useAutoCalculatedPlayerSpeedMultiplier)
        {
            CalculateSlopeSpeedMultiplier();
        }
        player.HorizontalVelocity = moveDir * player.CurrentSpeed;
    }

    private void MoveRelativeCamera()
    {
        if (input.MovementInput.magnitude == 0) return;

        float targetAngle = Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg
                          + cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(
            player.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            1 / currentTurnSmoothTime);

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        moveDir = Vector3.ProjectOnPlane(moveDir, sensors.BelowHit.normal).normalized;
        if (useAutoCalculatedPlayerSpeedMultiplier)
        {
            CalculateSlopeSpeedMultiplier();
        }
        player.HorizontalVelocity = moveDir * player.CurrentSpeed;
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
        player.InternalSpeedMultiplier = 0;
        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;
    }

    public void ExitState()
    {
        animator.SetFloat(speedHash, 0);
        animator.SetFloat(verticalSpeedHash, 0);
        animator.SetFloat(horizontalSpeedHash, 0);
        animator.SetBool(isSprintingHash, false);
    }

    private void SpeedControl()
    {
        UpdateMovementSpeed();
    }

    private void CalculateSlopeSpeedMultiplier()
    {
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(
        Vector3.down,
        sensors.BelowHit.normal
        );
        // Вычисляем косинус угла между направлением движения и направлением склона
        float dot = Vector3.Dot(player.HorizontalVelocity, projectedVelocity);

        // Теперь множитель скорости зависит от направления движения:
        // - dot > 0: движение вниз по склону — ускорение
        // - dot < 0: движение в гору — замедление
        // - dot ≈ 0: движение перпендикулярно склону — без изменений


        // Итоговый множитель скорости:
        var targetMultiplier = Mathf.Clamp(1f + dot * slopeAffectRate, 0.5f, 1.5f);
        player.ExternalSpeedMultiplier = Mathf.Lerp(
        player.ExternalSpeedMultiplier,
        targetMultiplier,
        Time.deltaTime * player.Acceleration);
    }

    private void DeltaAngle()
    {
        float currentYRotation = transform.eulerAngles.y;
        // Разница между текущим и предыдущим поворотом
        deltaY = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;

        // Обновляем предыдущий поворот
        lastYRotation = currentYRotation;
    }

    private void UpdateMovementSpeed()
    {
        IsSprinting = input.IsShifting && CanSprint;
        currentTurnSmoothTime = IsSprinting ? sprintTurnSmoothTime : normalTurnSmoothTime;

        float targetSpeedMultiplier = IsSprinting
            ? sprintSpeedMultiplier * MovementInput.magnitude
            : runSpeedMultiplier * MovementInput.magnitude;


        if (!isMoving)
        {
            player.InternalSpeedMultiplier = 0;
            return;

        }
        player.InternalSpeedMultiplier = Mathf.Lerp(
            player.InternalSpeedMultiplier,
            targetSpeedMultiplier,
            Time.deltaTime * (IsSprinting ? player.Acceleration * 1.5f : player.Acceleration));
    }
}
