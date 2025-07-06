using LOGIYGames;
using Unity.Netcode;
using UnityEngine;

public abstract class ActionContextBase : NetworkBehaviour
{
    [SerializeField] protected InputReader Input;

    [SerializeField] protected float Acceleration = 0f;
    [SerializeField] protected float Deceleration = 0f;
    [SerializeField] protected float InternalSpeedMultiplier = 0f;

    protected CharacterModule player;
    protected SensorsModule sensors;
    protected Animator animator;
    private float turnSmoothVelocity;
    protected float TurnSmoothTime = 10f;
    protected bool isMoving;
    protected float deltaY;
    float targetAngle = 0;
    public Vector2 MovementInput => Input.MoveInput;

    [SerializeField] public bool IsFocusing = true;
    [SerializeField] protected bool UseProjectionOnPlane = true;
    private float lastYRotation;

    [SerializeField] protected MotionType MotionType;

    protected virtual void Awake()
    {
        sensors = GetComponent<SensorsModule>();
        player = GetComponent<CharacterModule>();
        animator = GetComponent<Animator>();
        Input.EnableInputs();
    }

    public virtual void EnterState()
    {
        if (!IsOwner) return;

        ApplyRootMotion();

        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;

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

        //Rotate(); // если нужно, можно раскомментировать и переопределить Rotate
        Move();
        UpdateAnimations();
    }

    protected virtual void Move()
    {
        GetDeltaAngle();
        Vector3 moveDirection = RotateAndGetMovementDirection();
        if (UseProjectionOnPlane)
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, sensors.BelowHit.normal).normalized;
        }
        ChangeVelocity(moveDirection);
    }

    protected virtual void ChangeVelocity(Vector3 moveDirection)
    {
        if (MovementInput.magnitude > 0)
        {
            player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, player.Acceleration * Time.deltaTime);
        }
        else
        {
            player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, player.Deceleration * Time.deltaTime);
        }
        if (MotionType != MotionType.AnimatorController)
        {
            player.HorizontalVelocity = moveDirection * player.CurrentSpeed;

        }
        else
        {
            player.HorizontalVelocity = Vector3.zero;
        }
    }


    protected virtual Vector3 RotateAndGetMovementDirection()
    {
        if (IsFocusing)
        {
            return RotateAndGetDirectionAlongCamera();
        }
        else
        {


            return RotateAndGetDirectionRelativeCamera();
        }
    }


    protected virtual Vector3 RotateAndGetDirectionAlongCamera()
    {
        targetAngle = Camera.main.transform.eulerAngles.y;
        Rotate(targetAngle, TurnSmoothTime);
        return player.transform.right * MovementInput.x + player.transform.forward * MovementInput.y;
    }

    protected virtual Vector3 RotateAndGetDirectionRelativeCamera()
    {

        if (MovementInput.magnitude > 0)
        {
            targetAngle = Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg
                  + Camera.main.transform.eulerAngles.y;
        }

        if (MovementInput.magnitude != 0)
        {

            Rotate(targetAngle, TurnSmoothTime);
        }
        return player.transform.forward;
    }

    protected virtual void Rotate(float targetAngle, float turnSmoothTime = 0)
    {
        player.Rotate(Quaternion.Euler(0f, targetAngle, 0f), turnSmoothTime);
    }

    protected virtual void UpdateAnimations()
    {
        // Переопределять для обновления анимаций
    }

    protected virtual void GetDeltaAngle()
    {
        float currentYRotation = transform.eulerAngles.y;
        deltaY = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;
        lastYRotation = currentYRotation;
    }
}
