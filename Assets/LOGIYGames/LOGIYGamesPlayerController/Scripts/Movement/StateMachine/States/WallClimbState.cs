using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class WallClimbState : BaseMovementState
    {
        public WallClimbState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            _character.ResetVelocity();
            base.Enter();
            _character.MovementStrategy = new WallClimbMovement(_character.GetComponent<SensorsModule>(), _character);
            _character.RotationStrategy = new WallClimbRotaion(_character.GetComponent<SensorsModule>());
            _character.GetComponent<CharacterGravityModule>().UseGravity = false;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.ResetVelocity();
            _character.GetComponent<CharacterGravityModule>().UseGravity = true;

        }
    }
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
}
