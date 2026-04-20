using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMoveDirectionRotation : IRotationStrategy
    {
        Character Character;

        public ToMoveDirectionRotation(Character character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.LookRotation(Character.targetDirection);
        }
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

