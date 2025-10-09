using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public interface IState
    {
        void Enter();
        void Exit();

        void LogicUpdate();

        void PhysicsUpdate();
    }
    [Serializable]
    public abstract class BaseState : IState
    {
        public bool IsActiveState { get; protected set; }
        protected int isGroundedHash = Animator.StringToHash("IsGrounded");
        protected float Acceleration;
        protected float Deceleration;
        protected float TurnSmoothTime;
        protected bool moveBeforeRotation;
        protected float InternalSpeedMultiplier;
        protected Character Character;
        protected SensorsModule Sensors;
        protected Animator Animator;
        protected CharacterController CController;
        protected CharacterGravityModule CharacterGravity;

        protected float deltaYaw;
        protected Vector3 moveDirection;
        private float smoothTime = 0.3f;
        public Vector2 MovementInput => Character.MovementInput;

        private float lastYRotation;
        protected bool UseProjectionOnPlane = true;
        protected MotionType MotionType;

        protected int isMovingHash = Animator.StringToHash("IsMoving");
        protected int yawInputHash = Animator.StringToHash("Yaw Input");
        protected int speedHash = Animator.StringToHash("Speed");

        protected int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        protected int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        protected AnimationCurve locomotionCurve;
        protected float m_speed;
        protected BaseState(MovementStateDriver ctx, StateData stateData)
        {
            Acceleration = stateData.Acceleration;
            Deceleration = stateData.Deceleration;
            TurnSmoothTime = stateData.TurnSmothTime;
            MotionType = stateData.MotionType;
            locomotionCurve = stateData.AnimationCurve;
            m_speed = stateData.Speed;
            Sensors = ctx.GetComponent<SensorsModule>();
            Character = ctx.GetComponent<Character>();
            Animator = ctx.GetComponent<Animator>();
            CController = ctx.GetComponent<CharacterController>();
            CharacterGravity = ctx.GetComponent<CharacterGravityModule>();
        }

        public virtual void Enter()
        {
            InitializeMotionSystem();

            Character.Acceleration = Acceleration;
            Character.Deceleration = Deceleration;
            InternalSpeedMultiplier = m_speed;
        }


        public virtual void Exit()
        {
        }

        public virtual void LogicUpdate()
        {

        }

        public virtual void PhysicsUpdate()
        {
            Move();
            UpdateAnimations();
        }
        protected virtual void InitializeMotionSystem()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    Animator.applyRootMotion = false;
                    break;
                case MotionType.AnimatorController:
                    Animator.applyRootMotion = true;
                    break;
                default:
                    break;
            }
        }
        protected virtual void Move()
        {
            DebugDraw.DrawVector(Character.transform.position, Character.HorizontalVelocity, 1, 1, Color.blue, 0);
            GetDeltaAngle();
            GetMovementDirection();
            ChangeVelocity();
            ApplyMovement();
            HandleSteepWalls();
            Rotate();
        }
        protected virtual void ApplyMovement()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:

                    CController.Move(Character.HorizontalVelocity * Time.deltaTime);
                    break;
                case MotionType.AnimatorController:
                    break;
                default:
                    break;
            }

        }
        private void HandleSteepWalls()
        {
            Vector3 normal = Sensors.BelowHit.normal;


            if (!Sensors.IsValidSlope(normal) && CharacterGravity.VerticalVelocity < 0f)
            {
                // Направление соскальзывания = проекция вектора вниз на плоскость поверхности
                Vector3 slideDir = new Vector3(normal.x, -normal.y, normal.z);
                slideDir = Vector3.ProjectOnPlane(Vector3.down, normal).normalized;

                // добавляем смещение
                CController.Move(slideDir * -CharacterGravity.VerticalVelocity * Time.deltaTime);

            }
        }
        protected virtual void ChangeVelocity()
        {
            if (MovementInput.magnitude > 0)
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, moveDirection.normalized * Character.CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Character.Deceleration * Time.deltaTime);
            }

        }


        protected virtual void GetMovementDirection()
        {
            switch (CameraManager.Instance.CameraFocusingState)
            {
                case CameraFocusingState.FreeLook:
                    moveDirection = GetMovementDirectionRelativeCamera();
                    break;
                case CameraFocusingState.LookForward:
                    moveDirection = GetMovementDirectionAlongCamera();
                    break;
                case CameraFocusingState.Focus:
                    moveDirection = GetMovementDirectionAlongCamera();
                    break;
                default:
                    break;
            }
            if (UseProjectionOnPlane)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, Sensors.BelowHit.normal).normalized;
            }
        }


        protected virtual Vector3 GetMovementDirectionAlongCamera()
        {
            var fwd = Camera.main.transform.forward;
            fwd.y = 0;
            var rght = Camera.main.transform.right;
            rght.y = 0;
            return rght.normalized * MovementInput.x + fwd.normalized * MovementInput.y;
        }

        protected virtual Vector3 GetMovementDirectionRelativeCamera()
        {

            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

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
        protected virtual void Rotate()
        {
            if (!Character.IsUnderPlayerControl) return;
            switch (CameraManager.Instance.CameraFocusingState)
            {
                case CameraFocusingState.FreeLook:
                    RotateRelativeCamera();
                    break;
                case CameraFocusingState.LookForward:
                    RotateAlongCamera();
                    break;
                case CameraFocusingState.Focus:
                    RotateToTarget();
                    break;
                default:
                    break;
            }
        }

        protected virtual void RotateToTarget()
        {
            if (Character.Target == null)
            {
                return;
            }

            Character.RotateToPosition(Character.Target.position);
        }

        protected virtual void RotateRelativeCamera()
        {

            // Поворот вокруг оси Y, если есть движение
            if (MovementInput.magnitude > 0f)
            {
                var targetAngleY = Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                Character.Rotate(rotationY, TurnSmoothTime);
            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                Character.Rotate(rotationY, TurnSmoothTime);
            }

        }

        protected virtual void RotateAlongCamera()
        {
            var targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            Character.Rotate(targetRotation, TurnSmoothTime);
        }

        protected virtual void UpdateAnimations()
        {
            Animator.SetBool(isGroundedHash, Sensors.IsGrounded);
            float animatedspeed = 0;
            animatedspeed = locomotionCurve.Evaluate(Character.TotalSpeedMultiplier);
            bool isMoving = MovementInput.magnitude > 0;
            Animator.SetBool(isMovingHash, isMoving);
            Animator.SetFloat(yawInputHash, Mathf.Clamp(deltaYaw, -1, 1), smoothTime, Time.deltaTime);
            Animator.SetFloat(speedHash, animatedspeed, smoothTime, Time.deltaTime);
            switch (CameraManager.Instance.CameraFocusingState)
            {
                case CameraFocusingState.FreeLook:
                    {
                        Animator.SetFloat(verticalSpeedHash, animatedspeed);
                        Animator.SetFloat(horizontalSpeedHash, 0);
                    }
                    break;
                case CameraFocusingState.LookForward:
                    {
                        Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.HorizontalVelocity);
                        localVelocity.Normalize();
                        Animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed, smoothTime, Time.deltaTime);
                        Animator.SetFloat(verticalSpeedHash, localVelocity.z * animatedspeed, smoothTime, Time.deltaTime);
                    }
                    break;
                case CameraFocusingState.Focus:
                    {
                        Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.HorizontalVelocity);
                        localVelocity.Normalize();
                        Animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed, smoothTime, Time.deltaTime);
                        Animator.SetFloat(verticalSpeedHash, localVelocity.z * animatedspeed, smoothTime, Time.deltaTime);
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void GetDeltaAngle()
        {
            float currentYRotation = Character.transform.eulerAngles.y;
            deltaYaw = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;
            lastYRotation = currentYRotation;
        }
    }
    public abstract class TimedCooldownState : BaseState
    {
        protected TimedCooldownState(MovementStateDriver ctx, TimedCooldownStateData stateData) : base(ctx, stateData)
        {
            ActiveStateTime = stateData.ActiveStateTime;
            CooldownTime = stateData.CooldownStateTime;
            ActiveStateTimeCoundown = new CountdownTimer(ActiveStateTime);
            CooldownTimeCountdown = new CountdownTimer(CooldownTime);

            ActiveStateTimeCoundown.OnTimerStart += () => IsActiveState = true;
            ActiveStateTimeCoundown.OnTimerStop += () => IsActiveState = false;

            CooldownTimeCountdown.OnTimerStart += () => IsOnCooldown = true;
            CooldownTimeCountdown.OnTimerStop += () => IsOnCooldown = false;
        }
        protected float ActiveStateTime;
        protected float CooldownTime;
        protected CountdownTimer ActiveStateTimeCoundown;
        protected CountdownTimer CooldownTimeCountdown;
        public bool IsOnCooldown { get; protected set; }

        public override void Enter()
        {
            base.Enter();
            ActiveStateTimeCoundown.Start();
        }
        public override void Exit()
        {
            base.Exit();
            CooldownTimeCountdown.Start();
        }

        public virtual bool CanBeExecuted()
        {
            return !IsActiveState && !IsOnCooldown;
        }

    }

    public abstract class JumpState : TimedCooldownState
    {
        protected JumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            JumpVerticalForce = stateData.VerticalJumpForce;
            JumpPlanarForce = stateData.PlanarJumpForce;
        }
        protected float JumpVerticalForce;
        protected float JumpPlanarForce;
    }
    public abstract class ContinuousState : BaseState
    {
        public ContinuousState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            IsActiveState = true;
        }
        public override void Exit()
        {
            base.Exit();
            IsActiveState = false;
        }

    }
    public class RunState : ContinuousState
    {
        public RunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson && CameraManager.Instance.CurentCameraController.FOV != 60)
            {
                CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 60f, Time.deltaTime);
            }
        }
    }
    public class SprintState : ContinuousState
    {
        public SprintState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
        public override void LogicUpdate()
        {
            if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson && CameraManager.Instance.CurentCameraController.FOV != 90)
            {
                CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 90f, Time.deltaTime);
            }
        }
        public override void Enter()
        {
            base.Enter();
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
    //public abstract class GroundedState : BaseState
    //{
    //    protected GroundedState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //    }
    //    private bool useAutoCalculatedPlayerSpeedMultiplier = true;

    //    protected float slopeAffectRate = 0.2f;
    //    Vector3 projectedVelocity;
    //    protected override void ChangeVelocity()
    //    {
    //        base.ChangeVelocity();
    //        if (useAutoCalculatedPlayerSpeedMultiplier)
    //        {
    //            CalculateSlopeSpeedMultiplier();
    //        }
    //    }
    //    private void CalculateSlopeSpeedMultiplier()
    //    {
    //        projectedVelocity = Vector3.ProjectOnPlane(
    //        Vector3.down,
    //        Sensors.BelowHit.normal
    //        );
    //        // Вычисляем косинус угла между направлением движения и направлением склона
    //        float dot = Vector3.Dot(moveDirection, projectedVelocity);

    //        // Теперь множитель скорости зависит от направления движения:
    //        // - dot > 0: движение вниз по склону — ускорение
    //        // - dot < 0: движение в гору — замедление
    //        // - dot ≈ 0: движение перпендикулярно склону — без изменений


    //        // Итоговый множитель скорости:
    //        var targetMultiplier = Mathf.Clamp(1f + dot * slopeAffectRate, 0.5f, 1.5f);
    //        Character.ExternalSpeedMultiplier = Mathf.Lerp(
    //        Character.ExternalSpeedMultiplier,
    //        targetMultiplier,
    //        Time.deltaTime * Character.Acceleration);
    //    }
    //}
    public class FallingState : ContinuousState
    {
        public FallingState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }


        int fallingHash = Animator.StringToHash("IsFalling");

        public override void Enter()
        {
            base.Enter();
            Animator?.SetBool(fallingHash, true);
        }
        public override void Exit()
        {
            base.Exit();
            Animator?.SetBool(fallingHash, false);
        }
    }
    public class LandingState : TimedCooldownState
    {
        public LandingState(MovementStateDriver ctx, TimedCooldownStateData stateData) : base(ctx, stateData)
        {
        }
        int LandingHash = Animator.StringToHash("Landing");
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(LandingHash, 0.05f);
        }
    }
    public class GroundJumpState : JumpState
    {
        public GroundJumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {

        }

        private void Jump()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    CharacterGravity.VerticalVelocity = Mathf.Sqrt(JumpVerticalForce * -2f * Physics.gravity.y);
                    if (MovementInput.magnitude > 0)
                    {
                        Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

                        Vector3 cam = Camera.main.transform.forward;

                        Character.HorizontalVelocity += Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement * Character.TotalSpeedMultiplier * JumpPlanarForce;
                    }
                    break;
                default:
                    break;
            }
        }
        public override void Enter()
        {
            base.Enter();
            Animator?.CrossFade("Jump", 0.05f);
            Jump();
        }
    }
    public class CrouchState : ContinuousState
    {
        public CrouchState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            InitializeHeightValues();
        }
        int isCrouchingHash = Animator.StringToHash("IsCrouching");
        private float crouchHeightMultiplier = 0.5f;
        public float CrouchHeight { get; private set; }
        public float StandingHeight { get; private set; }

        private void InitializeHeightValues()
        {
            StandingHeight = CController.height;
            CrouchHeight = StandingHeight * crouchHeightMultiplier;
        }
        public override void LogicUpdate()
        {

        }
        public override void Enter()
        {
            base.Enter();
           // Character.Height = CrouchHeight;
            Animator.SetBool(isCrouchingHash,true);
        }
        public override void Exit()
        {
            base.Exit();
          //  Character.Height = StandingHeight;
            Animator.SetBool(isCrouchingHash, false);
        }
    }
    public class RollState : JumpState
    {
        public RollState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        int RollingHash = Animator.StringToHash("Rolling");
        protected override void Rotate()
        {
            return;
        }
        protected override void ChangeVelocity()
        {
            Character.HorizontalVelocity = Vector3.zero;
        }
        public override void Enter()
        {
            Animator.CrossFade(RollingHash, 0.05f);
            base.Enter();
        }

    }
    public class WallrunState : ContinuousState
    {
        public WallrunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            wallRunGravityMultiplier = 0;
            useWallCliping = true;
        }
        private float wallRunGravityMultiplier;
        private bool useWallCliping;
        Vector3 normal;
        Vector3 magnit => -normal;

        public bool CanWallRun()
        {
            return (Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight)
                        && !Sensors.IsGrounded
                        && MovementInput.y > 0;
            //&& !Sensors.IsObstacleLegsFront
            //&& Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 60;
        }
        protected override void GetMovementDirection()
        {

            normal = Sensors.IsObstacleLegsRight ? Sensors.LegsRightHit.normal : Sensors.LegsLeftHit.normal;

            Vector3 wallAlong = Vector3.Cross(normal, Character.transform.up).normalized;
            if ((Character.transform.forward - wallAlong).magnitude > (Character.transform.forward + wallAlong).magnitude)
            {
                wallAlong = -wallAlong;
            }

            moveDirection = wallAlong;
        }
        protected override void Move()
        {
            base.Move();
            if (useWallCliping)
            {
                Magnit();

            }
        }

        private void Magnit()
        {
            CController.Move(magnit * Time.deltaTime);

        }

        protected override void ChangeVelocity()
        {
            base.ChangeVelocity();
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(moveDirection, TurnSmoothTime);
        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.VerticalVelocity = 0;
            CharacterGravity.UseGravity = false;
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
        }
    }
    public class ClimbWallState : ContinuousState
    {
        public ClimbWallState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            useWallCliping = true;
        }
        int isOnWallHash = Animator.StringToHash("IsOnWall");
        private bool useWallCliping;
        Vector3 normal;
        Vector3 magnit => -normal;
        private void Magnit()
        {
            CController.Move(magnit * Time.deltaTime);

        }
        protected override void GetMovementDirection()
        {
            normal = Sensors.LegsFrontHit.normal;

            // Проекция вектора "вверх" персонажа на плоскость стены
            Vector3 wallAlongUp = Vector3.ProjectOnPlane(Character.transform.up, normal).normalized;

            // Проекция вектора "вправо" персонажа на ту же плоскость
            Vector3 wallAlongRight = Vector3.ProjectOnPlane(Character.transform.right, normal).normalized;

            // Пример комбинированного направления (можно изменить по задаче)
            moveDirection = wallAlongUp * MovementInput.y + wallAlongRight * MovementInput.x;
        }
        protected override void Move()
        {
            base.Move();
            if (useWallCliping)
            {
                Magnit();

            }
        }
        protected override void UpdateAnimations()
        {
            var animatedspeed = locomotionCurve.Evaluate(Character.TotalSpeedMultiplier);
            Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.HorizontalVelocity);
            localVelocity.Normalize();
            Animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed, 0.1f, Time.deltaTime);
            Animator.SetFloat(verticalSpeedHash, localVelocity.y * animatedspeed, 0.1f, Time.deltaTime);

        }
        protected override void ChangeVelocity()
        {
            base.ChangeVelocity();
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(-Sensors.LegsFrontHit.normal, 0);
        }
        public bool CanClimbWall()
        {
            return Sensors.LegsFrontHit.collider?.tag == "ClimbableWall";
            //&& Vector3.Angle(Character.transform.forward, -Sensors.LegsFrontHit.normal) < 60
            //&& Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 30;
        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.UseGravity = false;
            Animator.SetBool(isOnWallHash, true);

        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            CharacterGravity.VerticalVelocity = 0;
            Animator.SetBool(isOnWallHash, false);
        }
    }
    public class WallJumpState : JumpState
    {
        public WallJumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        bool IsWallJumpBackward;
        public void WallJump()
        {
            Vector3 wallNormal;

            if (Sensors.IsObstacleLegsFront)
            {
                CharacterGravity.VerticalVelocity = Mathf.Sqrt(JumpVerticalForce * -2 * Physics.gravity.y);
                Character.HorizontalVelocity = Sensors.LegsFrontHit.normal * JumpPlanarForce;
                Animator.CrossFade("WallJump", 0.05f);
            }
            else
            {
                if (Sensors.IsObstacleLegsRight)
                {
                    wallNormal = Sensors.LegsRightHit.normal;
                }
                else
                {
                    wallNormal = Sensors.LegsLeftHit.normal;
                }
                IsWallJumpBackward = false;
                CharacterGravity.VerticalVelocity = Mathf.Sqrt(JumpVerticalForce * -2 * Physics.gravity.y);
                Character.HorizontalVelocity = wallNormal * JumpPlanarForce + Character.transform.forward * JumpPlanarForce;
            }

        }
        protected override void Rotate()
        {

        }
        public override void Enter()
        {
            base.Enter();
            WallJump();
        }
    }
    public class SlideState : ContinuousState
    {
        private float requiredSpeedMultiplierToSlip;
        private float SlideSlopeAngleLimit;
        int isSlidingHash = Animator.StringToHash("IsSliding");
        public SlideState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
        protected override void GetMovementDirection()
        {
            Vector3 lookDirection = new Vector3(Character.HorizontalVelocity.x, 0f, Character.HorizontalVelocity.z);
            moveDirection = new Vector3(Sensors.BelowHit.normal.x, 0f, Sensors.BelowHit.normal.z).normalized;
        }
        protected override void ChangeVelocity()
        {

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(
            Vector3.down,
            Sensors.BelowHit.normal
                );
            //Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier, Time.deltaTime * Character.Acceleration);
            Character.HorizontalVelocity += projectedVelocity.normalized * Time.deltaTime * m_speed;
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(Character.HorizontalVelocity, 8);
        }
        public override void Enter()
        {
            Animator.SetBool(isSlidingHash, true);
            Character.HorizontalVelocity = Character.HorizontalVelocity / 2;
            base.Enter();
        }
        public override void Exit()
        {
            Animator.SetBool(isSlidingHash, false);
            base.Exit();
        }
    }
    public class SlipState : JumpState
    {
        public SlipState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        int SlipJumpHash = Animator.StringToHash("SlipJump");
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(SlipJumpHash, 0.05f);

        }
        protected override void GetMovementDirection()
        {
            moveDirection = Character.transform.forward;
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(moveDirection, TurnSmoothTime);
        }
    }
    public class DashState : JumpState
    {
        public DashState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        int dashHash = Animator.StringToHash("Dash");
        private void Dash()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    if (MovementInput.magnitude > 0)
                    {
                        Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

                        Vector3 cam = Camera.main.transform.forward;

                        Character.HorizontalVelocity += Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement * JumpPlanarForce;
                    }
                    break;
                default:
                    break;
            }
        }
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(dashHash, 0.05f);
            Dash();
        }
    }
    public class FlyState : ContinuousState
    {
        public FlyState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }

        int isFlyingHash = Animator.StringToHash("IsFlying");
        protected override void GetMovementDirection()
        {

            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

            Transform cam = Camera.main.transform;

            // Берём направления камеры
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            // Нормализуем, чтобы избежать случайных ускорений
            camForward.Normalize();
            camRight.Normalize();

            // Формируем итоговое направление в пространстве камеры
            Vector3 move = (camRight * movement.x) + (camForward * movement.z);
            moveDirection = move.normalized;
        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.VerticalVelocity = 0;
            CharacterGravity.UseGravity = false;
            Animator.SetBool(isFlyingHash, true);
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            Animator.SetBool(isFlyingHash, false);
        }
    }
    public class SwimState : ContinuousState
    {
        public SwimState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
        int isSwimmingHash = Animator.StringToHash("IsSwimming");
        protected override void GetMovementDirection()
        {

            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

            Transform cam = Camera.main.transform;

            // Берём направления камеры
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            // Нормализуем, чтобы избежать случайных ускорений
            camForward.Normalize();
            camRight.Normalize();

            // Формируем итоговое направление в пространстве камеры
            Vector3 move = (camRight * movement.x) + (camForward * movement.z);
            moveDirection = move.normalized;
        }
        protected override void RotateRelativeCamera()
        {
            if (MovementInput.magnitude > 0f)
            {
                Quaternion cameraRotation = Camera.main.transform.rotation;
                Character.Rotate(cameraRotation, TurnSmoothTime);
            }
            else
            {
                var targetAngleY = Character.transform.eulerAngles.y;
                Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                Character.Rotate(rotationY, TurnSmoothTime);
            }
        }
        protected override void RotateAlongCamera()
        {
            if (MovementInput.magnitude > 0f)
            {
                Quaternion cameraRotation = Camera.main.transform.rotation;
                Character.Rotate(cameraRotation, TurnSmoothTime);
            }
            else
            {
                var targetAngle = Camera.main.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                Character.Rotate(targetRotation, TurnSmoothTime);
            }
        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.VerticalVelocity = 0;
            CharacterGravity.UseGravity = false;
            Animator.SetBool(isSwimmingHash, true);
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            Animator.SetBool(isSwimmingHash, false);
        }
    }
    public class LedgeHangingState : ContinuousState
    {
        public LedgeHangingState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            useWallCliping = true;
        }
        int isLedgeHangingHash = Animator.StringToHash("IsOnWall");
        Vector3 normal;
        Vector3 magnit => -normal;
        Transform ledge = null;
        bool useWallCliping;
        protected override void Rotate()
        {
            Character.RotateToDirection(-normal);
        }
        protected override void GetMovementDirection()
        {
            normal = Sensors.ForeheadFrontHit.normal;

            Vector3 wallAlong = Vector3.Cross(normal, Character.transform.up);
            moveDirection = (wallAlong * Character.CurrentSpeed * MovementInput.x + magnit);
        }
        private void Magnit()
        {
            CController.Move(magnit * Time.deltaTime);

        }
        protected override void Move()
        {
            base.Move();
            if (useWallCliping)
            {
                Magnit();

            }
        }
        protected override void UpdateAnimations()
        {
            var animatedspeed = locomotionCurve.Evaluate(Character.TotalSpeedMultiplier);
            Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.HorizontalVelocity);
            localVelocity.Normalize();
            Animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed, 0.1f, Time.deltaTime);
            Animator.SetFloat(verticalSpeedHash, 0);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            AlignCharacterToLedge();
        }
        void AlignCharacterToLedge()
        {
            if (Sensors.ForeheadFrontHit.collider == null || Sensors.ForeheadFrontHit.collider?.tag != "Ledge")
            {
                return;
            }
            // Получаем центр объекта Ledge в мировых координатах
            Vector3 ledgeCenter = Sensors.ForeheadFrontHit.collider.bounds.center;
            float deltaY = ledgeCenter.y - Sensors.headFrontOrigin.y;
            var desiredPos = new Vector3(Character.transform.position.x, Character.transform.position.y + deltaY, Character.transform.position.z);
            Character.transform.position = desiredPos;
        }
        public override void Enter()
        {
            base.Enter();
            AlignCharacterToLedge();
            CharacterGravity.VerticalVelocity = 0;
            CharacterGravity.UseGravity = false;
            Animator.SetBool(isLedgeHangingHash, true);
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            Animator.SetBool(isLedgeHangingHash, false);
        }
    }
}
