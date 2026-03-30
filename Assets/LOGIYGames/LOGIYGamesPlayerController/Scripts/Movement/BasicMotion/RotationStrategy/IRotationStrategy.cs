using UnityEngine;

namespace LOGIYGames
{
    public interface IRotationStrategy
    {

        public Quaternion GetRotation();
    }
    //public class WallrunState : ContinuousState
    //{
    //    public WallrunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //        wallRunGravityMultiplier = 0;
    //        useWallCliping = true;
    //    }
    //    private float wallRunGravityMultiplier;
    //    private bool useWallCliping;
    //    Vector3 normal;
    //    Vector3 magnit => -normal;

    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //                    && (Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight)
    //                    && !Sensors.IsGrounded
    //                    && MovementInput.y > 0
    //                    && !Sensors.IsObstacleLegsFront
    //                    && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 60;
    //    }
    //    protected override void GetMovementDirection()
    //    {

    //        normal = Sensors.IsObstacleLegsRight ? Sensors.LegsRightHit.normal : Sensors.LegsLeftHit.normal;

    //        Vector3 wallAlong = Vector3.Cross(normal, Character.transform.up).normalized;
    //        if ((Character.transform.forward - wallAlong).magnitude > (Character.transform.forward + wallAlong).magnitude)
    //        {
    //            wallAlong = -wallAlong;
    //        }

    //        moveDirection = wallAlong;
    //    }
    //    protected override void Move()
    //    {
    //        base.Move();
    //        if (useWallCliping)
    //        {
    //            Magnit();

    //        }
    //    }

    //    private void Magnit()
    //    {
    //        CController.Move(magnit * Time.deltaTime);

    //    }
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(moveDirection, TurnSmoothTime);
    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //    }
    //}
    //public class ClimbWallState : ContinuousState
    //{
    //    public ClimbWallState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //        useWallCliping = false;
    //        InitializeHeightValues();
    //    }
    //    int isOnWallHash = Animator.StringToHash("IsOnWall");
    //    private bool useWallCliping;
    //    private float hangHeightMultiplier = 0.75f;
    //    public float BraceHangHeight { get; private set; }
    //    public float StandingHeight { get; private set; }

    //    private void InitializeHeightValues()
    //    {
    //        StandingHeight = CController.Height;
    //        BraceHangHeight = StandingHeight * hangHeightMultiplier;
    //    }
    //    Vector3 normal;
    //    Vector3 magnit => -normal;
    //    bool hasRightWall;
    //    bool hasLeftWall;
    //    bool hasUpWall;
    //    bool hasDownWall;
    //    private void Magnit()
    //    {
    //        CController.Move(magnit * Time.deltaTime);

    //    }
    //    protected override void GetMovementDirection()
    //    {
    //        normal = Sensors.LegsFrontHit.normal;



    //        // Проекции осей персонажа на плоскость стены
    //        Vector3 wallAlongUp = Vector3.ProjectOnPlane(Character.transform.up, normal).normalized;
    //        Vector3 wallAlongRight = Vector3.ProjectOnPlane(Character.transform.right, normal).normalized;

    //        // Рассчитываем движение, но учитываем ограничения по стенам
    //        Vector3 horizontalDir = Vector3.zero;
    //        Vector3 verticalDir = Vector3.zero;

    //        if (MovementInput.x > 0f && hasRightWall)         // вправо — только если есть стена справа
    //            horizontalDir = wallAlongRight * MovementInput.x;
    //        else if (MovementInput.x < 0f && hasLeftWall)     // влево — только если есть стена слева
    //            horizontalDir = wallAlongRight * MovementInput.x;

    //        if (MovementInput.y > 0f && hasUpWall)
    //            verticalDir = wallAlongUp * MovementInput.y;
    //        else if (MovementInput.y < 0f && hasDownWall)
    //            verticalDir = wallAlongUp * MovementInput.y;
    //        moveDirection = horizontalDir + verticalDir;
    //    }
    //    protected override void Move()
    //    {
    //        if (!hasLeftWall && MovementInput.x < 0) return;
    //        if (!hasRightWall && MovementInput.x > 0) return;
    //        if (!hasDownWall && MovementInput.y < 0) return;
    //        if (!hasUpWall && MovementInput.y > 0) return;

    //        base.Move();

    //    }
    //    public override void PhysicsUpdate()
    //    {
    //        base.PhysicsUpdate();
    //        if (useWallCliping)
    //        {
    //            Magnit();

    //        }
    //    }
    //    public override void LogicUpdate()
    //    {
    //        base.LogicUpdate();
    //        hasRightWall = Sensors.ForeheadFrontHit.collider != null &&
    //                            Sensors.ForeheadFrontHit.collider.CompareTag("ClimbableWall");
    //        hasLeftWall = Sensors.ForeheadLeftFrontHit.collider != null &&
    //                           Sensors.ForeheadLeftFrontHit.collider.CompareTag("ClimbableWall");
    //        hasUpWall = Sensors.ForeheadAboveFrontHit.collider != null &&
    //                Sensors.ForeheadAboveFrontHit.collider.CompareTag("ClimbableWall");
    //        hasDownWall = Sensors.LegsFrontHit.collider != null &&
    //                           Sensors.LegsFrontHit.collider.CompareTag("ClimbableWall");
    //    }
    //    protected override void UpdateAnimations()
    //    {
    //        // Проверяем наличие стен по бокам
    //        bool hasRightWall = Sensors.ForeheadFrontHit.collider != null &&
    //                            Sensors.ForeheadFrontHit.collider.CompareTag("ClimbableWall");
    //        bool hasLeftWall = Sensors.ForeheadLeftFrontHit.collider != null &&
    //                           Sensors.ForeheadLeftFrontHit.collider.CompareTag("ClimbableWall");
    //        bool hasUpWall = Sensors.ForeheadAboveFrontHit.collider != null &&
    //                Sensors.ForeheadAboveFrontHit.collider.CompareTag("ClimbableWall");
    //        bool hasDownWall = Sensors.LegsFrontHit.collider != null &&
    //                           Sensors.LegsFrontHit.collider.CompareTag("ClimbableWall");

    //        // Получаем локальную скорость
    //        Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.Velocity);

    //        // Ограничиваем анимацию бокового движения, если рядом нет стены
    //        float horizontal = localVelocity.x;
    //        float vertical = localVelocity.y;

    //        if (horizontal > 0f && !hasRightWall)
    //            horizontal = 0f;
    //        else if (horizontal < 0f && !hasLeftWall)
    //            horizontal = 0f;

    //        if (vertical > 0f && !hasUpWall)
    //            vertical = 0f;
    //        else if (vertical < 0f && !hasDownWall)
    //            vertical = 0f;
    //        // Применяем к Animator
    //        Animator.SetFloat(horizontalSpeedHash, horizontal * Character.TotalSpeedMultiplier, 0.1f, Time.deltaTime);
    //        Animator.SetFloat(verticalSpeedHash, vertical * Character.TotalSpeedMultiplier, 0.1f, Time.deltaTime);
    //    }
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(-Sensors.LegsFrontHit.normal, 0);
    //    }
    //    public override bool CanBeExecuted()
    //    {

    //        if (Character.FocusingState == FocusingState.Focus)
    //        {
    //            return base.CanBeExecuted() && Sensors.LegsFrontHit.collider?.tag == "ClimbableWall"
    //                && Vector3.Angle(Character.transform.forward, -Sensors.LegsFrontHit.normal) < 60
    //                && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 30;
    //        }
    //        else
    //        {
    //            return base.CanBeExecuted() && Sensors.LegsFrontHit.collider?.tag == "ClimbableWall";
    //        }

    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        CharacterGravity.UseGravity = false;
    //        CharacterGravity.Velocity = Vector3.zero;
    //        Character.ResetMotion();
    //        Animator.SetBool(isOnWallHash, true);
    //        Character.Height = BraceHangHeight;
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //        Character.Height = StandingHeight;
    //        Animator.SetBool(isOnWallHash, false);
    //    }
    //}
    //public class WallJumpState : JumpState
    //{
    //    public WallJumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    public void WallJump()
    //    {
    //        Vector3 velocity = Vector3.zero;
    //        switch (MotionType)
    //        {
    //            case MotionType.CharacterController:
    //                Vector3 wallNormal;
    //                if (Sensors.IsObstacleLegsFront)
    //                {
    //                    velocity = Sensors.LegsFrontHit.normal * JumpPlanarForce + Character.transform.up * JumpVerticalForce;
    //                    Animator.CrossFade("WallJump", 0.05f);
    //                }
    //                else
    //                {
    //                    if (Sensors.IsObstacleLegsRight)
    //                    {
    //                        wallNormal = Sensors.LegsRightHit.normal;
    //                    }
    //                    else
    //                    {
    //                        wallNormal = Sensors.LegsLeftHit.normal;
    //                    }
    //                    velocity = wallNormal * JumpPlanarForce + Character.transform.forward * JumpPlanarForce + Character.transform.up * JumpVerticalForce;
    //                }
    //                CharacterGravity.Velocity = Character.transform.up * Mathf.Sqrt(JumpVerticalForce * -2 * Physics.gravity.y);
    //                break;
    //            case MotionType.AnimatorController:
    //                if (Sensors.IsObstacleLegsFront)
    //                {
    //                    Animator.CrossFade("WallJump", 0.05f);
    //                }
    //                else
    //                {
    //                    if (Sensors.IsObstacleLegsRight)
    //                    {
    //                    }
    //                    else
    //                    {
    //                    }
    //                }
    //                break;
    //            default:
    //                break;

    //        }

    //        Character.Velocity += velocity;

    //    }
    //    protected override void Rotate()
    //    {

    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        WallJump();
    //    }
    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //            && Character.JumpPressed;
    //    }
    //}
    //public class SlideState : ContinuousState
    //{
    //    int isSlidingHash = Animator.StringToHash("IsSliding");
    //    int isSlidingFacingHash = Animator.StringToHash("IsSlidingFacingDown");
    //    public SlideState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    protected override void UpdateAnimations()
    //    {
    //        if (Character.transform.InverseTransformDirection(Character.Velocity).y < 0 && Character.Velocity.magnitude > 1)
    //        {
    //            if (Vector3.Angle(new Vector3(Character.Velocity.x, 0, Character.Velocity.z), Character.transform.forward) < 10)
    //            {
    //                Animator.SetBool(isSlidingFacingHash, true);
    //            }
    //            else
    //            {
    //                Animator.SetBool(isSlidingFacingHash, false);
    //            }
    //        }
    //        base.UpdateAnimations();
    //        Animator.SetBool(isSlidingHash, true);
    //    }
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(new Vector3(Character.Velocity.x, 0, Character.Velocity.z), TurnSmoothTime);
    //    }
    //    public override void Enter()
    //    {
    //        Animator.SetBool(isSlidingHash, true);
    //        base.Enter();
    //    }
    //    public override void Exit()
    //    {
    //        Animator.SetBool(isSlidingHash, false);
    //        Animator.SetBool(isSlidingFacingHash, false);
    //        base.Exit();
    //    }
    //}
    //public class SlipState : JumpState
    //{
    //    public SlipState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    int SlipJumpHash = Animator.StringToHash("SlipJump");
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        Animator.CrossFade(SlipJumpHash, 0.05f);
    //        Character.Velocity += Character.transform.forward * JumpPlanarForce;
    //    }
    //    protected override void GetMovementDirection()
    //    {
    //        moveDirection = Character.transform.forward;
    //    }
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(moveDirection, TurnSmoothTime);
    //    }
    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //            && Character.CrouchPressed
    //            && Character.MovementInput.y > 0;
    //    }
    //}
    //public class FlyState : ContinuousState
    //{
    //    public FlyState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //    }

    //    int isFlyingHash = Animator.StringToHash("IsFlying");
    //    protected override void GetMovementDirection()
    //    {
    //        if (CameraManager.Instance.CameraPerspectiveType != CameraPerspectiveType.Top_Down)
    //        {
    //            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

    //            Transform cam = Camera.main.transform;

    //            // Берём направления камеры
    //            Vector3 camForward = cam.forward;
    //            Vector3 camRight = cam.right;

    //            // Нормализуем, чтобы избежать случайных ускорений
    //            camForward.Normalize();
    //            camRight.Normalize();

    //            // Формируем итоговое направление в пространстве камеры
    //            Vector3 move = (camRight * movement.x) + (camForward * movement.z);
    //            moveDirection = move.normalized;
    //        }
    //        else
    //        {
    //            base.GetMovementDirection();
    //        }

    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //        Animator.SetBool(isFlyingHash, true);
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //        Animator.SetBool(isFlyingHash, false);
    //    }
    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //            && Character.BlockPressed;
    //    }
    //}
    //public class SwimState : ContinuousState
    //{
    //    public SwimState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    int isSwimmingHash = Animator.StringToHash("IsSwimming");
    //    protected override void GetMovementDirection()
    //    {
    //        if (CameraManager.Instance.CameraPerspectiveType != CameraPerspectiveType.Top_Down)
    //        {
    //            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

    //            Transform cam = Camera.main.transform;

    //            // Берём направления камеры
    //            Vector3 camForward = cam.forward;
    //            Vector3 camRight = cam.right;

    //            // Нормализуем, чтобы избежать случайных ускорений
    //            camForward.Normalize();
    //            camRight.Normalize();

    //            // Формируем итоговое направление в пространстве камеры
    //            Vector3 move = (camRight * movement.x) + (camForward * movement.z);
    //            moveDirection = move.normalized;
    //        }
    //        else
    //        {
    //            base.GetMovementDirection();
    //        }
    //    }
    //    protected override void RotateRelativeCamera()
    //    {
    //        if (MovementInput.magnitude > 0f)
    //        {
    //            Character.RotateToDirection(Character.Velocity, TurnSmoothTime);
    //        }
    //        else
    //        {
    //            var targetAngleY = Character.transform.eulerAngles.y;
    //            Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
    //            Character.Rotate(rotationY, TurnSmoothTime);
    //        }
    //    }
    //    protected override void RotateAlongCamera()
    //    {
    //        if (MovementInput.magnitude > 0f)
    //        {
    //            Quaternion cameraRotation = Camera.main.transform.rotation;
    //            Character.Rotate(cameraRotation, TurnSmoothTime);
    //        }
    //        else
    //        {
    //            var targetAngle = Camera.main.transform.eulerAngles.y;
    //            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
    //            Character.Rotate(targetRotation, TurnSmoothTime);
    //        }
    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //        Animator.SetBool(isSwimmingHash, true);
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //        Animator.SetBool(isSwimmingHash, false);
    //    }
    //}
    //public class LedgeHangingState : ContinuousState
    //{
    //    public LedgeHangingState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //        useWallCliping = true;
    //        InitializeHeightValues();
    //    }
    //    int isLedgeHangingHash = Animator.StringToHash("IsOnWall");
    //    public float BraceHangHeight { get; private set; }
    //    public float StandingHeight { get; private set; }
    //    private float hangHeightMultiplier = 0.75f;
    //    private void InitializeHeightValues()
    //    {
    //        StandingHeight = CController.Height;
    //        BraceHangHeight = StandingHeight * hangHeightMultiplier;
    //    }
    //    Vector3 normal;
    //    Vector3 magnit => -normal;
    //    Transform ledge = null;
    //    bool useWallCliping;
    //    bool hasRightWall;
    //    bool hasLeftWall;
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(-normal);
    //    }
    //    protected override void GetMovementDirection()
    //    {
    //        normal = Sensors.ForeheadFrontHit.normal;



    //        // Проекции осей персонажа на плоскость стены
    //        Vector3 wallAlongRight = Vector3.ProjectOnPlane(Character.transform.right, normal).normalized;

    //        // Рассчитываем движение, но учитываем ограничения по стенам
    //        Vector3 horizontalDir = Vector3.zero;

    //        if (MovementInput.x > 0f && hasRightWall)         // вправо — только если есть стена справа
    //            horizontalDir = wallAlongRight * MovementInput.x;
    //        else if (MovementInput.x < 0f && hasLeftWall)     // влево — только если есть стена слева
    //            horizontalDir = wallAlongRight * MovementInput.x;

    //        moveDirection = horizontalDir;
    //    }
    //    private void Magnit()
    //    {
    //        CController.Move(magnit * Time.deltaTime);

    //    }
    //    protected override void Move()
    //    {
    //        if (!hasLeftWall && MovementInput.x < 0) return;
    //        if (!hasRightWall && MovementInput.x > 0) return;
    //        base.Move();
    //    }
    //    protected override void UpdateAnimations()
    //    {

    //        // Получаем локальную скорость
    //        Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.Velocity);

    //        // Ограничиваем анимацию бокового движения, если рядом нет стены
    //        float horizontal = localVelocity.x;

    //        if (horizontal > 0f && !hasRightWall)
    //            horizontal = 0f;
    //        else if (horizontal < 0f && !hasLeftWall)
    //            horizontal = 0f;
    //        // Применяем к Animator
    //        Animator.SetFloat(horizontalSpeedHash, horizontal * Character.TotalSpeedMultiplier, 0.1f, Time.deltaTime);
    //        Animator.SetFloat(verticalSpeedHash, 0, 0.1f, Time.deltaTime);
    //    }
    //    public override void LogicUpdate()
    //    {
    //        base.LogicUpdate();

    //        hasRightWall = Sensors.ForeheadRightFrontHit.collider != null &&
    //                            Sensors.ForeheadRightFrontHit.collider.CompareTag("Ledge");
    //        hasLeftWall = Sensors.ForeheadLeftFrontHit.collider != null &&
    //                           Sensors.ForeheadLeftFrontHit.collider.CompareTag("Ledge");
    //    }
    //    public override void PhysicsUpdate()
    //    {
    //        base.PhysicsUpdate();
    //        if (useWallCliping)
    //        {
    //            Magnit();

    //        }
    //        AlignCharacterToLedge();
    //    }
    //    void AlignCharacterToLedge()
    //    {
    //        if (Sensors.ForeheadFrontHit.collider == null || Sensors.ForeheadFrontHit.collider?.tag != "Ledge")
    //        {
    //            return;
    //        }
    //        // Получаем центр объекта Ledge в мировых координатах
    //        Vector3 ledgeCenter = Sensors.ForeheadFrontHit.collider.bounds.center;
    //        float deltaY = ledgeCenter.y - Sensors.headFrontOrigin.y;
    //        var desiredPos = new Vector3(Character.transform.position.x, Character.transform.position.y + deltaY, Character.transform.position.z);
    //        Character.transform.position = desiredPos;
    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        AlignCharacterToLedge();
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //        Character.Height = BraceHangHeight;
    //        Animator.SetBool(isLedgeHangingHash, true);
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //        Animator.SetBool(isLedgeHangingHash, false);
    //        Character.Height = StandingHeight;
    //    }
    //}
    //public class LedgeHangUpState : TimedCooldownState
    //{
    //    public LedgeHangUpState(MovementStateDriver ctx, TimedCooldownStateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    int bracedHangUpHash = Animator.StringToHash("BracedHangUp");
    //    Vector3 start;
    //    Vector3 target;
    //    protected override void Rotate()
    //    {

    //    }
    //    public override void PhysicsUpdate()
    //    {
    //        base.PhysicsUpdate();
    //        switch (MotionType)
    //        {
    //            case MotionType.CharacterController:
    //                CController.transform.position = Vector3.Lerp(CController.transform.position, target, m_speed * Time.deltaTime);
    //                break;
    //            case MotionType.AnimatorController:

    //                break;
    //            default:
    //                break;
    //        }

    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        Animator.CrossFade(bracedHangUpHash, 0.05f);
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //        CController.enabled = false;
    //        start = CController.transform.position;
    //        target = start + Character.transform.up * (CController.Height + 0.1f) + Character.transform.forward * (CController.Radius + 0.1f);
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CController.enabled = true;
    //        CharacterGravity.UseGravity = true;
    //        if (!Sensors.IsGrounded)
    //        {
    //            CController.transform.position = CController.transform.position + CController.transform.forward * 0.25f;

    //        }
    //    }
    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted() && !Sensors.IsObstacleAboveHeadFront;
    //    }
    //}
    //public class JumpDown : JumpState
    //{
    //    public JumpDown(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    int jumpingDownHash = Animator.StringToHash("JumpingDown");
    //    int RunningJumpHash = Animator.StringToHash("RunningJump");
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        if (Character.SprintPressed)
    //        {
    //            Animator.CrossFade(RunningJumpHash, 0.05f);
    //            Character.Velocity += Character.transform.forward * JumpPlanarForce;
    //            CharacterGravity.Velocity = Character.transform.up * Mathf.Sqrt(JumpVerticalForce * -2 * Physics.gravity.y);
    //        }
    //        else
    //        {
    //            Animator.CrossFade(jumpingDownHash, 0.05f);
    //            Character.Velocity += Character.transform.forward * JumpPlanarForce;
    //        }
    //    }
    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //            && !Sensors.IsObstacleKneesFrontDown
    //            && !Sensors.IsObstacleLegsFront;
    //    }
    //}

}

