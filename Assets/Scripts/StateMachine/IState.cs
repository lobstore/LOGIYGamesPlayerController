using LOGIYGames.CharacterCore;
using System;
using UnityEditor.Overlays;
using UnityEngine;

namespace LOGIYGames
{
    public interface IMovementStrategy {

        public Vector3 GetMovementDirection();
    }
    public interface IRotationStrategy
    {

        public Quaternion GetRotation();
    }

    public class CameraRelativeRotation : IRotationStrategy
    {
        Character Character { get; set; }
        public CameraRelativeRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            if (Character.MovementInput.magnitude > 0f)
            {

                var targetAngleY = Mathf.Atan2(Character.MovementInput.x, Character.MovementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                return rotationY;

            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                return rotationY;
            }
        }
    }

    public class CameraAlongRotation : IRotationStrategy
    {
        Character Character { get; set; }
        public CameraAlongRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            var targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            return targetRotation;
        }
    }

    public class ToMousePointRotation : IRotationStrategy
    {
        Character Character { get; set; }
        public ToMousePointRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = (hitPoint - Character.transform.position);
                direction.y = 0f; // чтобы не заваливался вверх/вниз

                if (direction != Vector3.zero)
                {
                    return Quaternion.LookRotation(direction);
                }
                else
                {
                    return Quaternion.identity;
                }
            }
            else
            {
                return Quaternion.identity;
            }
        }
    }

    public class CameraRelativeMovement : IMovementStrategy
    {
        Character Character { get; set; }
        public CameraRelativeMovement(Character character)
        {
            Character = character;
        }
        public Vector3 GetMovementDirection()
        {
            Vector3 movement = new Vector3(Character.MovementInput.x, 0, Character.MovementInput.y);

            Transform cam = Camera.main.transform;

            // Берём направления камеры
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            // Обнуляем вертикальную составляющую, чтобы не было движения вверх/вниз
            camForward.y = 0f;
            camRight.y = 0f;

            // Нормализуем, чтобы избежать ускорения при наклоне камеры
            camForward.Normalize();
            camRight.Normalize();

            // Рассчитываем направление движения относительно камеры
            Vector3 move = (camRight * movement.x) + (camForward * movement.z);

            return move.normalized;
        }
    }

    public class CameraAlongMovement : IMovementStrategy
    {
        Character Character { get; set; }
        public CameraAlongMovement(Character character)
        {
            Character = character;
        }
        public Vector3 GetMovementDirection()
        {
            var fwd = Camera.main.transform.forward;
            fwd.y = 0;
            var rght = Camera.main.transform.right;
            rght.y = 0;
            return rght.normalized * Character.MovementInput.x + fwd.normalized * Character.MovementInput.y;
        }
    }

    public interface IState
    {
        void Enter();
        void Exit();

        void LogicUpdate();

        void LateUpdate();
        void PhysicsUpdate();
    }
    [Serializable]
    public abstract class BaseState : IState
    {

        public IMovementStrategy m_CurrentMovementStrategy;
        public IRotationStrategy m_CurrentRotationStrategy;
        CameraAlongMovement m_CameraAlongMovement;
        CameraRelativeRotation m_cameraRelativeRotation;
        CameraAlongRotation m_CameraAlongRotation;

        protected Character m_character;
        protected StateData m_data;
        protected BaseState(MovementStateDriver ctx, StateData stateData)
        {
            m_data = new();
            m_character = ctx.Character;
            m_data.StateName = stateData.StateName;
            m_data.Acceleration = stateData.Acceleration;
            m_data.Deceleration = stateData.Deceleration;
            m_data.TurnSmothTime = stateData.TurnSmothTime;
            m_data.Speed = stateData.Speed;

            m_CameraAlongMovement = new(m_character);
            m_cameraRelativeRotation = new(m_character);
            m_CameraAlongRotation = new(m_character);

            m_CurrentMovementStrategy = m_CameraAlongMovement;
            m_CurrentRotationStrategy = m_cameraRelativeRotation;
        }
        public virtual void Enter()
        {
            m_character.Acceleration = m_data.Acceleration; 
            m_character.Deceleration = m_data.Deceleration;
            m_character.TurnSmoothTime = m_data.TurnSmothTime;
            m_character.SpeedMultiplier = m_data.Speed;
            Debug.Log("Entered " + GetType());
        }
        public virtual void Exit()
        {
        }
        public virtual void LogicUpdate()
        {


            if (Input.GetKey(KeyCode.Mouse1)||CameraManager.Instance.CameraPerspectiveType==CameraPerspectiveType.FirstPerson)
            {
                m_CurrentRotationStrategy = m_CameraAlongRotation;

            }
            else if(!Input.GetKey(KeyCode.Mouse1))
            {
                m_CurrentRotationStrategy = m_cameraRelativeRotation;
            }

        }

        public virtual void LateUpdate()
        {
    
        }

        public virtual void PhysicsUpdate()
        {
            m_character.Rotate(m_CurrentRotationStrategy.GetRotation(), m_character.TurnSmoothTime);
            m_character.Move(m_CurrentMovementStrategy.GetMovementDirection());

        }

    }
    public class JumpState : BaseState
    {
        JumpStateData m_stateData;
        public JumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            m_stateData = new();
            m_stateData.PlanarJumpForce=stateData.PlanarJumpForce;
            m_stateData.VerticalJumpForce =stateData.VerticalJumpForce;
        }
        public override void Enter()
        {
            base.Enter();
            m_character.JumpVerticalForce = m_stateData.VerticalJumpForce;
            m_character.JumpPlanarForce = m_stateData.PlanarJumpForce;
            m_character.Jump();
        }
    }
    public class RunState : BaseState
    {
        public RunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class WalkState : BaseState
    {
        public WalkState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class IdleState : BaseState
    {
        public IdleState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class SprintState : BaseState
    {
        public SprintState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class FallingState : BaseState
    {
        public FallingState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }

    }
    public class CrouchState : BaseState
    {
        public CrouchState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            StandingHeight = m_character.Height;
            CrouchHeight = StandingHeight * 0.5f;
        }
        protected float StandingHeight;
        protected float CrouchHeight;

        public override void Enter()
        {
            base.Enter();
            m_character.Height = CrouchHeight;
        }
        
        public override void Exit()
        {
            base.Exit();
            m_character.Height = StandingHeight;
        }
    }
    public class RollState : BaseState
    {
        JumpStateData m_StateData;
        public RollState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            m_StateData = new();
            m_StateData.VerticalJumpForce = stateData.VerticalJumpForce;
            m_StateData.PlanarJumpForce = stateData.PlanarJumpForce;

        }
        public override void Enter()
        {
            base.Enter();
            m_character.JumpVerticalForce = m_StateData.VerticalJumpForce;
            m_character.JumpPlanarForce = m_StateData.PlanarJumpForce;
            m_character.Roll();
        }
    }

    public class LandingState : BaseState
    {
        public LandingState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class StopState : BaseState
    {
        public StopState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {

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
    //public class DashState : JumpState
    //{
    //    public DashState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    int StandingDodgeForward = Animator.StringToHash("StandingDodgeForward");
    //    int StandingDodgeRight = Animator.StringToHash("StandingDodgeRight");
    //    int StandingDodgeLeft = Animator.StringToHash("StandingDodgeLeft");
    //    int StandingDodgeBack = Animator.StringToHash("StandingDodgeBack");

    //    private void Dash()
    //    {
    //        Vector3 inputDir = new Vector3(MovementInput.x, 0, MovementInput.y);
    //        if (inputDir.sqrMagnitude < 0.01f)
    //            return;

    //        // --- Определяем направление камеры на плоскости ---
    //        Vector3 camForward = Camera.main.transform.forward;
    //        camForward.y = 0;
    //        camForward.Normalize();

    //        Vector3 camRight = Camera.main.transform.right;
    //        camRight.y = 0;
    //        camRight.Normalize();

    //        // --- Преобразуем ввод в мировое направление относительно камеры ---
    //        Vector3 worldDir = (camForward * MovementInput.y + camRight * MovementInput.x).normalized;

    //        // --- Добавляем импульс для рывка ---
    //        Character.Velocity += worldDir * JumpPlanarForce;

    //        // --- Определяем направление относительно персонажа ---
    //        Vector3 localDir = Character.transform.InverseTransformDirection(worldDir).normalized;

    //        // --- Выбираем анимацию ---
    //        if (Character.FocusingState == FocusingState.FreeLook)
    //        {
    //            Animator.CrossFade(StandingDodgeForward, 0.05f);
    //        }
    //        else if (Character.FocusingState is FocusingState.Focus or FocusingState.Focus)
    //        {
    //            float forwardDot = Vector3.Dot(localDir, Vector3.forward);
    //            float rightDot = Vector3.Dot(localDir, Vector3.right);

    //            // Сравниваем проекции, чтобы определить направление
    //            if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
    //            {
    //                if (forwardDot > 0)
    //                    Animator.CrossFade(StandingDodgeForward, 0.05f);
    //                else
    //                    Animator.CrossFade(StandingDodgeBack, 0.05f);
    //            }
    //            else
    //            {
    //                if (rightDot > 0)
    //                    Animator.CrossFade(StandingDodgeRight, 0.05f);
    //                else
    //                    Animator.CrossFade(StandingDodgeLeft, 0.05f);
    //            }
    //        }
    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        Dash();
    //    }
    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //            && MovementInput.magnitude > 0
    //            && Character.SprintPressed;
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

