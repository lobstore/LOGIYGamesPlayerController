using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public enum MotionType
    {
        AnimatorController,
        CharacterController
    }
    public enum FocusingState
    {
        FreeLook,
        Focus,
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
        public bool IsActiveState { get; protected set; }
        protected int isGroundedHash = Animator.StringToHash("IsGrounded");
        protected float Acceleration;
        protected float Deceleration;
        protected float TurnSmoothTime;
        protected bool moveBeforeRotation = true;
        protected float StateInternalSpeedMultiplier;
        protected Character Character;
        protected SensorsModule Sensors;
        protected Animator Animator;
        protected GenericControllerWrapper CController;
        protected CharacterGravityModule CharacterGravity;
        protected StaminaModel Stamina;
        protected float StaminaCost;
        protected float MotionSpeed;

        protected float deltaYaw;
        protected Vector3 moveDirection;
        private float smoothTime = 0.1f;
        public Vector2 MovementInput => Character.MovementInput;

        private float lastYRotation;
        protected bool UseProjectionOnPlane = true;
        protected MotionType MotionType;


        protected int isMovingHash = Animator.StringToHash("IsMoving");
        protected int yawInputHash = Animator.StringToHash("Yaw Signed Angle");
        protected int yawAbsHash = Animator.StringToHash("Yaw Absolute Angle");
        protected int turnAngleHash = Animator.StringToHash("TurnAngle");
        protected int animationSpeedHash = Animator.StringToHash("AnimationSpeed");
        protected int SpeedHash = Animator.StringToHash("Speed");
        protected int IsStairsUpHash = Animator.StringToHash("IsOnStair");
        protected int isAimingHash = Animator.StringToHash("IsAiming");

        protected int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        protected int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        protected float m_speed;
        protected bool RotateByRootMotionOnly;
        private bool isDebugDraw = true;
        private bool HandlingSlope = true;

        //private StateMachine SubStateMachine = new();
        //private IState defaultState;
        protected BaseState(MovementStateDriver ctx, StateData stateData)
        {
            Acceleration = stateData.Acceleration;
            Deceleration = stateData.Deceleration;
            TurnSmoothTime = stateData.TurnSmothTime;
            m_speed = stateData.Speed;

            MotionType = stateData.MotionType;
            HandlingSlope = stateData.HandlingSlope;
            RotateByRootMotionOnly = stateData.RotateByRootMotionOnly;
            UseProjectionOnPlane = stateData.UseProjectionOnPlane;

            StaminaCost = stateData.StaminaCost;
            Sensors = ctx.Sensors;
            Character = ctx.Character;
            Animator = ctx.Animator;
            CController = ctx.ControllerWrapper;
            CharacterGravity = ctx.GravityModule;
            Stamina = ctx.StaminaModel;

            
            
        }
        public virtual void Enter()
        {
            InitializeMotionSystem();
            Character.Acceleration = Acceleration;
            Character.Deceleration = Deceleration;
            StateInternalSpeedMultiplier = m_speed;
        }
        public void SetDefaultSubState(IState state)
        {
            ///defaultState = state;
            //SubStateMachine.SetState(defaultState);
        }
        public void AddSubstateTransition(IState from, IState to, IPredicate contidion)
        {
            //SubStateMachine.AddTransition(from, to, contidion);
        }
        public void AddAnySubstateTransition(IState to, IPredicate contidion)
        {
            //SubStateMachine.AddAnyTransition(to, contidion);
        }
        public virtual void Exit()
        {
            //SubStateMachine.ChangeState(defaultState);
        }
        protected virtual void Aim()
        {
            if (Character.BlockPressed)
            {
                Animator.SetBool(isAimingHash, true);
                CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 40f, Time.deltaTime*5);
            }
            else
            {
                CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 60f, Time.deltaTime*5);
                Animator.SetBool(isAimingHash, false);

            }
        }
        public virtual void LogicUpdate()
        {
            //SubStateMachine.Update();
            if (Character.TotalSpeedMultiplier > 1.5)
            {
                MotionSpeed = 1 + Character.TotalSpeedMultiplier - 1.5f;

            }
            else
            {
                MotionSpeed = 1;
            }
            switch (CameraManager.Instance.CameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    Character.FocusingState = FocusingState.Focus;
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    Character.FocusingState = Character.FocusPressed || Character.BlockPressed ? FocusingState.Focus : FocusingState.FreeLook;
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    Character.FocusingState = FocusingState.Focus;
                    break;
                case CameraPerspectiveType.Top_Down:
                    Character.FocusingState = Character.FocusPressed || Character.BlockPressed ? FocusingState.Focus : FocusingState.FreeLook;
                    RotateByRootMotionOnly = false;
                    break;
                default:
                    break;
            }
        }

        public virtual void LateUpdate()
        {
            //SubStateMachine.LateUpdate();
            UpdateAnimations();
            Aim();
            if (isDebugDraw)
            {
                Debug();
            }
        }

        private void Debug()
        {
            float baseLength = 2;
            DebugDraw.DrawArrow(Character.transform.position, CharacterGravity.Velocity, Color.purple);
            DebugDraw.DrawArrow(Character.transform.position, Character.Velocity.normalized * Character.TotalSpeedMultiplier * baseLength, Color.green);
            DebugDraw.DrawArrow(Character.transform.position, moveDirection.normalized * StateInternalSpeedMultiplier * Character.ExternalSpeedMultiplier * baseLength, Color.yellow);
            DebugDraw.DrawArrow(Character.transform.position, Animator.deltaPosition.normalized * Character.TotalSpeedMultiplier * baseLength, Color.whiteSmoke);
        }

        public virtual void PhysicsUpdate()
        {
            //SubStateMachine.FixedUpdate();
            ChangeVelocity();
            GetDeltaAngle();
            GetMovementDirection();
            Move();
            Rotate();

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
            if (HandlingSlope)
            {
                HandleSlope();
            }
            ApplyMovement();
        }
        protected virtual void ApplyMovement()
        {

            if (CController.enabled)
            {
                switch (MotionType)
                {
                    case MotionType.CharacterController:
                        if (UseProjectionOnPlane)
                        {
                            Character.Velocity = Vector3.ProjectOnPlane(Character.Velocity, Sensors.BelowHit.normal);
                        }
                        CController.Move(Character.Velocity * Time.deltaTime);

                        break;
                    case MotionType.AnimatorController:
                        var deltaPosition = Animator.deltaPosition;
                        var projectedDelta = Vector3.zero;
                        if (UseProjectionOnPlane)
                        {
                            var normal = Sensors.BelowHit.normal;
                            projectedDelta = Vector3.ProjectOnPlane(deltaPosition, normal);
                            float originalSpeed = deltaPosition.magnitude;
                            if (projectedDelta.sqrMagnitude > 0.0001f)
                            {
                                deltaPosition = projectedDelta.normalized * originalSpeed;
                            }
                            else
                            {
                                deltaPosition = Vector3.zero;
                            }
                        }
                        CController.MoveAndRotate(deltaPosition, Animator.deltaRotation);
                        break;
                    default:
                        break;
                }
            }

        }
        private void HandleSlope()
        {
            if (!Sensors.IsValidSlope())
            {
                Vector3 normal = Sensors.BelowHit.normal;
                // Направление соскальзывания = проекция вектора вниз на плоскость поверхности
                Vector3 slideDir = Vector3.zero;
                if (Character.transform.InverseTransformDirection(CharacterGravity.Velocity).y <= 0)
                {
                    var absAngle = Mathf.Abs(Sensors.GroundAngle);
                    float slideModifier = absAngle >= 90 ? 1f : Mathf.InverseLerp(0f, 90, absAngle);

                    //slideDir = Vector3.ProjectOnPlane(Character.Velocity + CharacterGravity.Velocity, normal);
                    slideDir = Vector3.ProjectOnPlane(CharacterGravity.Velocity, normal);
                    //Character.Velocity = Vector3.Lerp(Character.Velocity, slideDir, Time.deltaTime * slideModifier * 5);
                    CController.Move(slideDir *Time.deltaTime);
                }
            }
        }
        protected virtual void ChangeVelocity()
        {
            if (MovementInput.magnitude > 0)
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, StateInternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.Velocity = Vector3.Lerp(Character.Velocity, moveDirection.normalized * Character.CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);
                Character.Velocity = Vector3.Lerp(Character.Velocity, Vector3.zero, Character.Deceleration * Time.deltaTime);
            }

        }


        protected virtual void GetMovementDirection()
        {
            switch (Character.FocusingState)
            {
                case FocusingState.FreeLook:
                    moveDirection = GetMovementDirectionRelativeCamera();
                    break;
                case FocusingState.Focus:
                    moveDirection = GetMovementDirectionAlongCamera();
                    break;
                default:
                    break;
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
            if (moveBeforeRotation)
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
            else
            {
                return Character.transform.forward;
            }
        }
        protected virtual void Rotate()
        {
            if (!Character.IsUnderPlayerControl) return;
            if (CameraManager.Instance.CameraPerspectiveType != CameraPerspectiveType.Top_Down)
            {
                switch (Character.FocusingState)
                {
                    case FocusingState.FreeLook:
                        RotateRelativeCamera();
                        break;
                    case FocusingState.Focus:
                        RotateAlongCamera();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (Character.FocusingState)
                {
                    case FocusingState.FreeLook:
                        RotateRelativeCamera();
                        break;
                    case FocusingState.Focus:
                        RotateToMousePosition();
                        break;
                    default:
                        break;
                }

            }
        }
        protected virtual void RotateToMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = (hitPoint - Character.transform.position);
                direction.y = 0f; // чтобы не заваливался вверх/вниз

                if (direction != Vector3.zero)
                    Character.Rotate(Quaternion.LookRotation(direction), TurnSmoothTime);
            }
        }
        protected virtual void RotateRelativeCamera()
        {
            if (MovementInput.magnitude > 0f)
            {
                if (!RotateByRootMotionOnly)
                {

                    var targetAngleY = Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    Quaternion rotationY = Quaternion.Euler(0f, targetAngleY, 0f);
                    Character.Rotate(rotationY, TurnSmoothTime);
                }


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
            Character.Rotate(targetRotation, 0);
        }

        protected virtual void UpdateAnimations()
        {
            Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.Velocity).normalized;
            Animator.SetBool(isGroundedHash, Sensors.IsGrounded);
            bool isMoving = MovementInput.magnitude > 0;
            Animator.SetBool(isMovingHash, isMoving);

            Animator.SetFloat(yawInputHash, deltaYaw);
            Animator.SetFloat(turnAngleHash, deltaYaw, 1 / TurnSmoothTime, Time.deltaTime);
            Animator.SetFloat(SpeedHash, Character.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            Animator.SetFloat(yawAbsHash, Mathf.Abs(deltaYaw));
            Animator.SetFloat(animationSpeedHash, MotionSpeed, smoothTime, Time.deltaTime);
            Animator.SetBool(IsStairsUpHash, Sensors.IsStepUpAhead);
            switch (Character.FocusingState)
            {
                case FocusingState.FreeLook:
                    {
                        Animator.SetFloat(horizontalSpeedHash, 0, smoothTime, Time.deltaTime);
                        Animator.SetFloat(verticalSpeedHash, Character.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
                    }
                    break;
                case FocusingState.Focus:
                    {
                        Animator.SetFloat(horizontalSpeedHash, localVelocity.x * Character.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
                        Animator.SetFloat(verticalSpeedHash, localVelocity.z * Character.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void GetDeltaAngle()
        {
            if (Character.FocusingState == FocusingState.FreeLook && CameraManager.Instance.CameraPerspectiveType != CameraPerspectiveType.Top_Down)
            {

                float turnInput = Vector3.SignedAngle(Character.transform.forward, moveDirection, Vector3.up);
                deltaYaw = turnInput;

            }
            else
            {
                deltaYaw = 0;
            }
        }

        public virtual bool CanBeExecuted()
        {
            if (Stamina == null)
            {
                return true;
            }
            else
            {
                return Stamina.CurrentValue >= StaminaCost;

            }
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

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && !IsActiveState
                && !IsOnCooldown;
        }

    }
    public abstract class ContinuousState : BaseState
    {
        public ContinuousState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            Stamina.CurrentValue -= StaminaCost * Time.deltaTime;
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
    public abstract class JumpState : TimedCooldownState
    {
        protected JumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            JumpVerticalForce = stateData.VerticalJumpForce;
            JumpPlanarForce = stateData.PlanarJumpForce;
        }
        protected float JumpVerticalForce;
        protected float JumpPlanarForce;
        public override void Enter()
        {
            base.Enter();
            Stamina.CurrentValue -= StaminaCost;
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
    public class WalkState : ContinuousState
    {
        public WalkState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
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
    public class IdleState : ContinuousState
    {
        public IdleState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
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
            base.LogicUpdate();
            if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson && CameraManager.Instance.CurentCameraController.FOV != 90)
            {
                CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 90f, Time.deltaTime);
            }
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && Character.SprintPressed && Character.MovementInput.magnitude > 0;
        }
    }
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
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Sensors.IsGrounded && CharacterGravity.Velocity.y<3;
        }
        protected override void ChangeVelocity()
        {
            Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);
            Character.Velocity = Vector3.Lerp(Character.Velocity, Vector3.zero, Character.Deceleration * Time.deltaTime);
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
                    CharacterGravity.Velocity = Character.transform.up * Mathf.Sqrt(JumpVerticalForce * -2f * Physics.gravity.y);
                    if (MovementInput.magnitude > 0)
                    {
                        Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

                        Vector3 cam = Camera.main.transform.forward;

                        Character.Velocity += Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement * Character.TotalSpeedMultiplier * JumpPlanarForce;
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
        public override void Exit()
        {
            base.Exit();
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Character.JumpPressed;
        }

    }
    public class CrouchState : ContinuousState
    {
        public CrouchState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            InitializeHeightValues();
        }
        int isCrouchingHash = Animator.StringToHash("IsCrouching");
        private float crouchHeightMultiplier = 0.7f;
        public float CrouchHeight { get; private set; }
        public float StandingHeight { get; private set; }

        private void InitializeHeightValues()
        {
            StandingHeight = CController.Height;
            CrouchHeight = StandingHeight * crouchHeightMultiplier;
        }
        public override void Enter()
        {
            base.Enter();
            Character.Height = CrouchHeight;
            Animator.SetBool(isCrouchingHash, true);
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
        }
        public override void Exit()
        {
            base.Exit();
            Character.Height = StandingHeight;
            Animator.SetBool(isCrouchingHash, false);
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Character.CrouchPressed;
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
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(RollingHash, 0.05f);
            Character.Velocity += Character.transform.forward * JumpPlanarForce;
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Character.EvadePressed;
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

        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                        && (Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight)
                        && !Sensors.IsGrounded
                        && MovementInput.y > 0
                        && !Sensors.IsObstacleLegsFront
                        && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 60;
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
        protected override void Rotate()
        {
            Character.RotateToDirection(moveDirection, TurnSmoothTime);
        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.Velocity = Vector3.zero;
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
            useWallCliping = false;
            InitializeHeightValues();
        }
        int isOnWallHash = Animator.StringToHash("IsOnWall");
        private bool useWallCliping;
        private float hangHeightMultiplier = 0.75f;
        public float BraceHangHeight { get; private set; }
        public float StandingHeight { get; private set; }

        private void InitializeHeightValues()
        {
            StandingHeight = CController.Height;
            BraceHangHeight = StandingHeight * hangHeightMultiplier;
        }
        Vector3 normal;
        Vector3 magnit => -normal;
        bool hasRightWall;
        bool hasLeftWall;
        bool hasUpWall;
        bool hasDownWall;
        private void Magnit()
        {
            CController.Move(magnit * Time.deltaTime);

        }
        protected override void GetMovementDirection()
        {
            normal = Sensors.LegsFrontHit.normal;



            // Проекции осей персонажа на плоскость стены
            Vector3 wallAlongUp = Vector3.ProjectOnPlane(Character.transform.up, normal).normalized;
            Vector3 wallAlongRight = Vector3.ProjectOnPlane(Character.transform.right, normal).normalized;

            // Рассчитываем движение, но учитываем ограничения по стенам
            Vector3 horizontalDir = Vector3.zero;
            Vector3 verticalDir = Vector3.zero;

            if (MovementInput.x > 0f && hasRightWall)         // вправо — только если есть стена справа
                horizontalDir = wallAlongRight * MovementInput.x;
            else if (MovementInput.x < 0f && hasLeftWall)     // влево — только если есть стена слева
                horizontalDir = wallAlongRight * MovementInput.x;

            if (MovementInput.y > 0f && hasUpWall)
                verticalDir = wallAlongUp * MovementInput.y;
            else if (MovementInput.y < 0f && hasDownWall)
                verticalDir = wallAlongUp * MovementInput.y;
            moveDirection = horizontalDir + verticalDir;
        }
        protected override void Move()
        {
            if (!hasLeftWall && MovementInput.x < 0) return;
            if (!hasRightWall && MovementInput.x > 0) return;
            if (!hasDownWall && MovementInput.y < 0) return;
            if (!hasUpWall && MovementInput.y > 0) return;

            base.Move();

        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (useWallCliping)
            {
                Magnit();

            }
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            hasRightWall = Sensors.ForeheadFrontHit.collider != null &&
                                Sensors.ForeheadFrontHit.collider.CompareTag("ClimbableWall");
            hasLeftWall = Sensors.ForeheadLeftFrontHit.collider != null &&
                               Sensors.ForeheadLeftFrontHit.collider.CompareTag("ClimbableWall");
            hasUpWall = Sensors.ForeheadAboveFrontHit.collider != null &&
                    Sensors.ForeheadAboveFrontHit.collider.CompareTag("ClimbableWall");
            hasDownWall = Sensors.LegsFrontHit.collider != null &&
                               Sensors.LegsFrontHit.collider.CompareTag("ClimbableWall");
        }
        protected override void UpdateAnimations()
        {
            // Проверяем наличие стен по бокам
            bool hasRightWall = Sensors.ForeheadFrontHit.collider != null &&
                                Sensors.ForeheadFrontHit.collider.CompareTag("ClimbableWall");
            bool hasLeftWall = Sensors.ForeheadLeftFrontHit.collider != null &&
                               Sensors.ForeheadLeftFrontHit.collider.CompareTag("ClimbableWall");
            bool hasUpWall = Sensors.ForeheadAboveFrontHit.collider != null &&
                    Sensors.ForeheadAboveFrontHit.collider.CompareTag("ClimbableWall");
            bool hasDownWall = Sensors.LegsFrontHit.collider != null &&
                               Sensors.LegsFrontHit.collider.CompareTag("ClimbableWall");

            // Получаем локальную скорость
            Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.Velocity);

            // Ограничиваем анимацию бокового движения, если рядом нет стены
            float horizontal = localVelocity.x;
            float vertical = localVelocity.y;

            if (horizontal > 0f && !hasRightWall)
                horizontal = 0f;
            else if (horizontal < 0f && !hasLeftWall)
                horizontal = 0f;

            if (vertical > 0f && !hasUpWall)
                vertical = 0f;
            else if (vertical < 0f && !hasDownWall)
                vertical = 0f;
            // Применяем к Animator
            Animator.SetFloat(horizontalSpeedHash, horizontal * Character.TotalSpeedMultiplier, 0.1f, Time.deltaTime);
            Animator.SetFloat(verticalSpeedHash, vertical * Character.TotalSpeedMultiplier, 0.1f, Time.deltaTime);
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(-Sensors.LegsFrontHit.normal, 0);
        }
        public override bool CanBeExecuted()
        {

            if (Character.FocusingState == FocusingState.Focus)
            {
                return base.CanBeExecuted() && Sensors.LegsFrontHit.collider?.tag == "ClimbableWall"
                    && Vector3.Angle(Character.transform.forward, -Sensors.LegsFrontHit.normal) < 60
                    && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 30;
            }
            else
            {
                return base.CanBeExecuted() && Sensors.LegsFrontHit.collider?.tag == "ClimbableWall";
            }

        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.UseGravity = false;
            CharacterGravity.Velocity = Vector3.zero;
            Character.ResetMotion();
            Animator.SetBool(isOnWallHash, true);
            Character.Height = BraceHangHeight;
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            Character.Height = StandingHeight;
            Animator.SetBool(isOnWallHash, false);
        }
    }
    public class WallJumpState : JumpState
    {
        public WallJumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        public void WallJump()
        {
            Vector3 velocity = Vector3.zero;
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    Vector3 wallNormal;
                    if (Sensors.IsObstacleLegsFront)
                    {
                        velocity = Sensors.LegsFrontHit.normal * JumpPlanarForce + Character.transform.up * JumpVerticalForce;
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
                        velocity = wallNormal * JumpPlanarForce + Character.transform.forward * JumpPlanarForce + Character.transform.up * JumpVerticalForce;
                    }
                    CharacterGravity.Velocity = Character.transform.up * Mathf.Sqrt(JumpVerticalForce * -2 * Physics.gravity.y);
                    break;
                case MotionType.AnimatorController:
                    if (Sensors.IsObstacleLegsFront)
                    {
                        Animator.CrossFade("WallJump", 0.05f);
                    }
                    else
                    {
                        if (Sensors.IsObstacleLegsRight)
                        {
                        }
                        else
                        {
                        }
                    }
                    break;
                default:
                    break;

            }

            Character.Velocity += velocity;

        }
        protected override void Rotate()
        {

        }
        public override void Enter()
        {
            base.Enter();
            WallJump();
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Character.JumpPressed;
        }
    }
    public class SlideState : ContinuousState
    {
        int isSlidingHash = Animator.StringToHash("IsSliding");
        int isSlidingFacingHash = Animator.StringToHash("IsSlidingFacingDown");
        public SlideState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
        }
        protected override void UpdateAnimations()
        {
            if (Character.transform.InverseTransformDirection(Character.Velocity).y < 0 && Character.Velocity.magnitude > 1)
            {
                if (Vector3.Angle(new Vector3(Character.Velocity.x, 0, Character.Velocity.z), Character.transform.forward) < 10)
                {
                    Animator.SetBool(isSlidingFacingHash, true);
                }
                else
                {
                    Animator.SetBool(isSlidingFacingHash, false);
                }
            }
            base.UpdateAnimations();
            Animator.SetBool(isSlidingHash, true);
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(new Vector3(Character.Velocity.x, 0, Character.Velocity.z), TurnSmoothTime);
        }
        public override void Enter()
        {
            Animator.SetBool(isSlidingHash, true);
            base.Enter();
        }
        public override void Exit()
        {
            Animator.SetBool(isSlidingHash, false);
            Animator.SetBool(isSlidingFacingHash, false);
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
            Character.Velocity += Character.transform.forward * JumpPlanarForce;
        }
        protected override void GetMovementDirection()
        {
            moveDirection = Character.transform.forward;
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(moveDirection, TurnSmoothTime);
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Character.CrouchPressed
                && Character.MovementInput.y > 0;
        }
    }
    public class DashState : JumpState
    {
        public DashState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        int StandingDodgeForward = Animator.StringToHash("StandingDodgeForward");
        int StandingDodgeRight = Animator.StringToHash("StandingDodgeRight");
        int StandingDodgeLeft = Animator.StringToHash("StandingDodgeLeft");
        int StandingDodgeBack = Animator.StringToHash("StandingDodgeBack");

        private void Dash()
        {
            Vector3 inputDir = new Vector3(MovementInput.x, 0, MovementInput.y);
            if (inputDir.sqrMagnitude < 0.01f)
                return;

            // --- Определяем направление камеры на плоскости ---
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            // --- Преобразуем ввод в мировое направление относительно камеры ---
            Vector3 worldDir = (camForward * MovementInput.y + camRight * MovementInput.x).normalized;

            // --- Добавляем импульс для рывка ---
            Character.Velocity += worldDir * JumpPlanarForce;

            // --- Определяем направление относительно персонажа ---
            Vector3 localDir = Character.transform.InverseTransformDirection(worldDir).normalized;

            // --- Выбираем анимацию ---
            if (Character.FocusingState == FocusingState.FreeLook)
            {
                Animator.CrossFade(StandingDodgeForward, 0.05f);
            }
            else if (Character.FocusingState is FocusingState.Focus or FocusingState.Focus)
            {
                float forwardDot = Vector3.Dot(localDir, Vector3.forward);
                float rightDot = Vector3.Dot(localDir, Vector3.right);

                // Сравниваем проекции, чтобы определить направление
                if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
                {
                    if (forwardDot > 0)
                        Animator.CrossFade(StandingDodgeForward, 0.05f);
                    else
                        Animator.CrossFade(StandingDodgeBack, 0.05f);
                }
                else
                {
                    if (rightDot > 0)
                        Animator.CrossFade(StandingDodgeRight, 0.05f);
                    else
                        Animator.CrossFade(StandingDodgeLeft, 0.05f);
                }
            }
        }
        public override void Enter()
        {
            base.Enter();
            Dash();
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && MovementInput.magnitude > 0
                && Character.SprintPressed;
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
            if (CameraManager.Instance.CameraPerspectiveType != CameraPerspectiveType.Top_Down)
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
            else
            {
                base.GetMovementDirection();
            }

        }
        public override void Enter()
        {
            base.Enter();
            CharacterGravity.Velocity = Vector3.zero;
            CharacterGravity.UseGravity = false;
            Animator.SetBool(isFlyingHash, true);
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            Animator.SetBool(isFlyingHash, false);
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && Character.BlockPressed;
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
            if (CameraManager.Instance.CameraPerspectiveType != CameraPerspectiveType.Top_Down)
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
            else
            {
                base.GetMovementDirection();
            }
        }
        protected override void RotateRelativeCamera()
        {
            if (MovementInput.magnitude > 0f)
            {
                Character.RotateToDirection(Character.Velocity, TurnSmoothTime);
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
            CharacterGravity.Velocity = Vector3.zero;
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
            InitializeHeightValues();
        }
        int isLedgeHangingHash = Animator.StringToHash("IsOnWall");
        public float BraceHangHeight { get; private set; }
        public float StandingHeight { get; private set; }
        private float hangHeightMultiplier = 0.75f;
        private void InitializeHeightValues()
        {
            StandingHeight = CController.Height;
            BraceHangHeight = StandingHeight * hangHeightMultiplier;
        }
        Vector3 normal;
        Vector3 magnit => -normal;
        Transform ledge = null;
        bool useWallCliping;
        bool hasRightWall;
        bool hasLeftWall;
        protected override void Rotate()
        {
            Character.RotateToDirection(-normal);
        }
        protected override void GetMovementDirection()
        {
            normal = Sensors.ForeheadFrontHit.normal;



            // Проекции осей персонажа на плоскость стены
            Vector3 wallAlongRight = Vector3.ProjectOnPlane(Character.transform.right, normal).normalized;

            // Рассчитываем движение, но учитываем ограничения по стенам
            Vector3 horizontalDir = Vector3.zero;

            if (MovementInput.x > 0f && hasRightWall)         // вправо — только если есть стена справа
                horizontalDir = wallAlongRight * MovementInput.x;
            else if (MovementInput.x < 0f && hasLeftWall)     // влево — только если есть стена слева
                horizontalDir = wallAlongRight * MovementInput.x;

            moveDirection = horizontalDir;
        }
        private void Magnit()
        {
            CController.Move(magnit * Time.deltaTime);

        }
        protected override void Move()
        {
            if (!hasLeftWall && MovementInput.x < 0) return;
            if (!hasRightWall && MovementInput.x > 0) return;
            base.Move();
        }
        protected override void UpdateAnimations()
        {

            // Получаем локальную скорость
            Vector3 localVelocity = Character.transform.InverseTransformDirection(Character.Velocity);

            // Ограничиваем анимацию бокового движения, если рядом нет стены
            float horizontal = localVelocity.x;

            if (horizontal > 0f && !hasRightWall)
                horizontal = 0f;
            else if (horizontal < 0f && !hasLeftWall)
                horizontal = 0f;
            // Применяем к Animator
            Animator.SetFloat(horizontalSpeedHash, horizontal * Character.TotalSpeedMultiplier, 0.1f, Time.deltaTime);
            Animator.SetFloat(verticalSpeedHash, 0, 0.1f, Time.deltaTime);
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            hasRightWall = Sensors.ForeheadRightFrontHit.collider != null &&
                                Sensors.ForeheadRightFrontHit.collider.CompareTag("Ledge");
            hasLeftWall = Sensors.ForeheadLeftFrontHit.collider != null &&
                               Sensors.ForeheadLeftFrontHit.collider.CompareTag("Ledge");
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (useWallCliping)
            {
                Magnit();

            }
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
            CharacterGravity.Velocity = Vector3.zero;
            CharacterGravity.UseGravity = false;
            Character.Height = BraceHangHeight;
            Animator.SetBool(isLedgeHangingHash, true);
        }
        public override void Exit()
        {
            base.Exit();
            CharacterGravity.UseGravity = true;
            Animator.SetBool(isLedgeHangingHash, false);
            Character.Height = StandingHeight;
        }
    }
    public class LedgeHangUpState : TimedCooldownState
    {
        public LedgeHangUpState(MovementStateDriver ctx, TimedCooldownStateData stateData) : base(ctx, stateData)
        {
        }
        int bracedHangUpHash = Animator.StringToHash("BracedHangUp");
        Vector3 start;
        Vector3 target;
        protected override void Rotate()
        {

        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    CController.transform.position = Vector3.Lerp(CController.transform.position, target, m_speed * Time.deltaTime);
                    break;
                case MotionType.AnimatorController:

                    break;
                default:
                    break;
            }

        }
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(bracedHangUpHash, 0.05f);
            CharacterGravity.Velocity = Vector3.zero;
            CharacterGravity.UseGravity = false;
            CController.enabled = false;
            start = CController.transform.position;
            target = start + Character.transform.up * (CController.Height + 0.1f) + Character.transform.forward * (CController.Radius + 0.1f);
        }
        public override void Exit()
        {
            base.Exit();
            CController.enabled = true;
            CharacterGravity.UseGravity = true;
            if (!Sensors.IsGrounded)
            {
                CController.transform.position = CController.transform.position + CController.transform.forward * 0.25f;

            }
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted() && !Sensors.IsObstacleAboveHeadFront;
        }
    }
    public class JumpDown : JumpState
    {
        public JumpDown(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
        }
        int jumpingDownHash = Animator.StringToHash("JumpingDown");
        int RunningJumpHash = Animator.StringToHash("RunningJump");
        public override void Enter()
        {
            base.Enter();
            if (Character.SprintPressed)
            {
                Animator.CrossFade(RunningJumpHash, 0.05f);
                Character.Velocity += Character.transform.forward * JumpPlanarForce;
                CharacterGravity.Velocity = Character.transform.up * Mathf.Sqrt(JumpVerticalForce * -2 * Physics.gravity.y);
            }
            else
            {
                Animator.CrossFade(jumpingDownHash, 0.05f);
                Character.Velocity += Character.transform.forward * JumpPlanarForce;
            }
        }
        public override bool CanBeExecuted()
        {
            return base.CanBeExecuted()
                && !Sensors.IsObstacleKneesFrontDown
                && !Sensors.IsObstacleLegsFront;
        }
    }
    public class AimState :IState
    {
        int isAimingHash = Animator.StringToHash("IsAiming");

        public AimState(MovementStateDriver ctx)
        {
            Animator = ctx.Animator;
            Character = ctx.Character;
        }
        Animator Animator;
        Character Character;
        public void LogicUpdate()
        {
            Character.FocusingState = FocusingState.Focus;
            Animator.SetBool(isAimingHash, true);
        }

        public void Enter()
        {
            Animator.SetBool(isAimingHash, true);
        }
        public void Exit()
        {
            Animator.SetBool(isAimingHash, false);
        }
        public bool CanBeExecuted()
        {
            return Character.BlockPressed;
        }

        public void LateUpdate()
        {
           
        }

        public void PhysicsUpdate()
        {
 
        }
    }
}

