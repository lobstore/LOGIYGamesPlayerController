using LOGIYGames;
using Unity.Netcode;
using UnityEngine;

public abstract class ActionContextBase : NetworkBehaviour
{
    [SerializeField] protected InputReader input;
    [SerializeField] float animationSmoothTime = 0.3f;

    [SerializeField] protected float Acceleration = 10f;
    [SerializeField] protected float Deceleration = 1f;
    [SerializeField] protected float InternalSpeedMultiplier = 1;

    protected CharacterModule player;
    protected SensorsModule sensors;
    protected Animator animator;
    protected float targetAngle;
    protected Vector3 moveDirection;
    private float turnSmoothVelocity;
    protected float TurnSmoothTime = 10f;
    protected bool isMoving;
    protected float deltaY;
    public Vector2 MovementInput => input.MoveInput;

    public bool IsFocusing { get; private set; } = true;

    private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    private int yawInputHash = Animator.StringToHash("Yaw Input");
    private int speedHash = Animator.StringToHash("Speed");
    private int isMovingHash = Animator.StringToHash("IsMoving");
    private float lastYRotation;
    protected Vector3 moveDir;

    protected virtual void Awake()
    {
        sensors = GetComponent<SensorsModule>();
        player = GetComponent<CharacterModule>();
        animator = GetComponent<Animator>();
        input.EnableInputs();
    }

    [SerializeField] protected MotionType MotionType;

    public virtual void EnterState()
    {
        if (!IsOwner) return;
        if (MotionType == MotionType.AnimatorController)
        {
            animator.applyRootMotion = true;
        }
        else
        {
            animator.applyRootMotion = false;
        }
        player.InternalSpeedMultiplier = InternalSpeedMultiplier;
        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;

        UpdateAnimations();
    }
    public virtual void ExitState()
    {
        if (!IsOwner) return;
        UpdateAnimations();

    }
    public virtual void OnUpdate()
    {
        if (!IsOwner) return;

    }
    public virtual void OnFixedUpdate()
    {
        if (!IsOwner) return;
        Move();
        UpdateAnimations();
       // DebugInfo();
    }

    private void Move()
    {
        DeltaAngle();
        Rotate();
        ChangeVelocity();
    }

    protected virtual void ChangeVelocity()
    {
        float targetAngle = Camera.main.transform.eulerAngles.y;

        moveDir = player.transform.right * MovementInput.x + player.transform.forward * MovementInput.y;
        moveDir = Vector3.ProjectOnPlane(moveDir, sensors.BelowHit.normal).normalized;

        player.HorizontalVelocity = moveDir * player.CurrentSpeed;
    }
    protected virtual void Rotate()
    {
        float angle = Mathf.SmoothDampAngle(
        player.transform.eulerAngles.y,
        targetAngle,
        ref turnSmoothVelocity,
        1 / (TurnSmoothTime * 4));

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    protected virtual void UpdateAnimations()
    {
        isMoving = MovementInput.magnitude > 0;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetFloat(speedHash, player.TotalSpeedMultiplier, animationSmoothTime, Time.deltaTime);
        if (IsFocusing)
        {
            animator.SetFloat(verticalSpeedHash, MovementInput.y * Mathf.Clamp01( player.HorizontalVelocity.magnitude)* player.TotalSpeedMultiplier, animationSmoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, MovementInput.x * Mathf.Clamp01(player.HorizontalVelocity.magnitude) * player.TotalSpeedMultiplier, animationSmoothTime, Time.deltaTime);
        }
        else
        {
            animator.SetFloat(verticalSpeedHash, Mathf.Clamp01(player.HorizontalVelocity.magnitude) * player.TotalSpeedMultiplier, animationSmoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, 0);
        }
        animator.SetFloat(yawInputHash, Mathf.Clamp(deltaY, -1, 1), animationSmoothTime, Time.deltaTime);
    }
    private void DeltaAngle()
    {
        float currentYRotation = transform.eulerAngles.y;
        // Разница между текущим и предыдущим поворотом
        deltaY = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;

        // Обновляем предыдущий поворот
        lastYRotation = currentYRotation;
    }
    protected virtual void DebugInfo()
    {
        DebugDraw.DebugField(input, nameof(input));
        DebugDraw.DebugField(IsOwner, nameof(IsOwner));
        DebugDraw.DebugField(Acceleration, nameof(Acceleration));
        DebugDraw.DebugField(Deceleration, nameof(Deceleration));
        DebugDraw.DebugField(InternalSpeedMultiplier, nameof(InternalSpeedMultiplier));
        if (MovementInput.magnitude > 0)
        {
            DebugDraw.DebugField(MovementInput, nameof(MovementInput));
            DebugDraw.DebugField(moveDir, nameof(moveDir));

        }
    }
}
