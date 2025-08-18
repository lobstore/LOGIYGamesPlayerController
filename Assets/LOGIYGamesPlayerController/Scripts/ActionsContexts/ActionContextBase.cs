using LOGIYGames;
using Unity.Netcode;
using UnityEngine;

public abstract class ActionContextBase : MonoBehaviour
{
    [SerializeField] protected InputReader Input;

    [SerializeField] protected float Acceleration = 0f;
    [SerializeField] protected float Deceleration = 0f;
    protected float InternalSpeedMultiplier = 0f;
    [SerializeField] protected float TurnSmoothTime = 10f;

    protected CharacterModule player;
    protected SensorsModule sensors;
    protected Animator animator;

    protected bool isMoving;
    protected float deltaYaw;
    protected Vector3 moveDirection;
    public Vector2 MovementInput => Input.MoveInput;

    public bool CanMove { get => Input.PlayerInputsEnable; set => Input.PlayerInputsEnable = value; }

    [SerializeField] public bool IsFocusing = true;
    [SerializeField] protected bool UseProjectionOnPlane = true;
    private float lastYRotation;

    [SerializeField] public MotionType MotionType;

    protected virtual void Awake()
    {

        print("a");
        sensors = GetComponent<SensorsModule>();
        player = GetComponent<CharacterModule>();
        print("l");
        animator = GetComponent<Animator>();
        Input.PlayerInputsEnable=true;
    }

    public virtual void EnterState()
    {
       // if (!IsOwner) return;

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


        DebugDraw.DrawVector(transform.position, player.HorizontalVelocity, 1, 1, Color.blue, 0);
        ChangeVelocity();
        Rotate();
    }

    protected virtual void ChangeVelocity()
    {
        if (MovementInput.magnitude > 0)
        {
            player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, player.Acceleration * Time.deltaTime);
            player.HorizontalVelocity = Vector3.Lerp(player.HorizontalVelocity, moveDirection * player.CurrentSpeed, Acceleration * Time.fixedDeltaTime);
            
        }
        else
        {
            player.InternalSpeedMultiplier = Mathf.Lerp(player.InternalSpeedMultiplier, 0, player.Deceleration * Time.deltaTime);
            player.HorizontalVelocity = Vector3.Lerp(player.HorizontalVelocity, Vector3.zero, player.Deceleration * Time.fixedDeltaTime);

            //if (IsFocusing)
            //{
            //    player.HorizontalVelocity = GetMovementDirectionAlongCamera() * player.CurrentSpeed;
            //}
            //else
            //{
            //    var dir = Vector3.ProjectOnPlane(player.transform.forward, sensors.BelowHit.normal).normalized;
            //    player.HorizontalVelocity = dir * player.CurrentSpeed;
            //}


        }
    }


    protected virtual void GetMovementDirection()
    {
        if (IsFocusing)
        {
            moveDirection = GetMovementDirectionAlongCamera();
        }
        else
        {
            moveDirection = GetMovementDirectionRelativeCamera();
        }
        if (UseProjectionOnPlane)
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, sensors.BelowHit.normal).normalized;
        }
    }


    protected virtual Vector3 GetMovementDirectionAlongCamera()
    {
        return player.transform.right * MovementInput.x + player.transform.forward * MovementInput.y;
    }

    protected virtual Vector3 GetMovementDirectionRelativeCamera()
    {

        Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

        Vector3 cam = Camera.main.transform.forward;

        return Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement;

    }
    protected virtual void Rotate()
    {
        if (!IsFocusing)
        {
            RotateRelativeCamera();

        }
        else
        {
            RotateAlongCamera();
        }



    }

    private void RotateRelativeCamera()
    {
        if (moveDirection.magnitude > 0)
        {
            // Рассчитываем угол поворота по направлению движения
            var targetAngle = Mathf.Atan2(player.HorizontalVelocity.x, player.HorizontalVelocity.z) * Mathf.Rad2Deg;

            // Плавно поворачиваем объект в сторону этого угла
            player.Rotate(Quaternion.Euler(0f, targetAngle, 0f), TurnSmoothTime);
        }
    }
    protected float lastVerticalAngle = 0f;
    protected const float angleThreshold = 45f;
    private void RotateAlongCamera()
    {
        var targetAngle = Camera.main.transform.eulerAngles.y;
        player.Rotate(Quaternion.Euler(0f, targetAngle, 0f), TurnSmoothTime);

        // Получаем текущий вертикальный угол камеры (ось X)
        float currentVerticalAngle = Camera.main.transform.eulerAngles.y;

        // Считаем разницу углов с учётом перехода через 360/0 градусов
        float deltaAngle = Mathf.DeltaAngle(lastVerticalAngle, currentVerticalAngle);
        if (Mathf.Abs(deltaAngle) > angleThreshold)
        {
            if (deltaAngle > 0)
            {
                Debug.Log("RightTurnTriggered");
                animator.SetTrigger("IsRightTurning");
            }
            else
            {
                Debug.Log("LeftTurnTriggered");
                animator.SetTrigger("IsLeftTurning");
            }
            lastVerticalAngle = currentVerticalAngle; // сохраняем угол после срабатывания
        }

    }

    protected virtual void UpdateAnimations()
    {
        // Переопределять для обновления анимаций
    }

    protected virtual void GetDeltaAngle()
    {
        float currentYRotation = transform.eulerAngles.y;
        deltaYaw = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;
        lastYRotation = currentYRotation;
    }
}
