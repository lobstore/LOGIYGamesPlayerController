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
        int isGroundedHash = Animator.StringToHash("IsGrounded");
        protected float Acceleration = 0f;
        protected float Deceleration = 0f;
        protected float TurnSmoothTime = 10f;
        protected bool moveBeforeRotation;
        protected float InternalSpeedMultiplier = 0f;
        protected const float angleThreshold = 45f;
        protected Character Character;
        protected SensorsModule Sensors;
        protected Animator Animator;
        protected CharacterController CController;
        protected CharacterGravityModule CharacterGravity;
        protected bool isMoving;
        protected float deltaYaw;
        protected Vector3 moveDirection;
        public Vector2 MovementInput => Character.MovementInput;

        private float lastYRotation;
        protected bool UseProjectionOnPlane = true;
        protected MotionType MotionType;

        protected BaseState(MovementStateDriver ctx)
        {
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

            UpdateAnimations();
        }


        public virtual void Exit()
        {
            UpdateAnimations();
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
            if (moveBeforeRotation)
            {
                Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);
                Vector3 cam = Camera.main.transform.forward;
                return Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement;
            }
            else
            {
                return Character.transform.forward;
            }
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

        private void RotateToTarget()
        {
            if (Character.Target == null)
            {
                return;
            }

            Character.RotateToPosition(Character.Target.position);
        }

        private void RotateRelativeCamera()
        {
            if (MovementInput.magnitude > 0f)
            {
                // Рассчитываем угол поворота по направлению движения
                var targetAngle = Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                Character.Rotate(targetRotation, TurnSmoothTime);
            }
        }

        private void RotateAlongCamera()
        {
            var targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            Character.Rotate(targetRotation, TurnSmoothTime);
        }

        protected virtual void UpdateAnimations()
        {
            Animator.SetBool(isGroundedHash, Sensors.IsGrounded);
        }

        protected virtual void GetDeltaAngle()
        {
            float currentYRotation = Character.transform.eulerAngles.y;
            deltaYaw = Mathf.DeltaAngle(lastYRotation, currentYRotation) * Time.deltaTime * 10f;
            lastYRotation = currentYRotation;
        }
    }

    public class LocomotionState : GroundedState
    {
        private float smoothTime = 0.3f;
        protected float walkSpeed = 0.5f;
        protected float runSpeed = 1f;
        protected float sprintSpeed = 1.5f;
        private int isMovingHash = Animator.StringToHash("IsMoving");
        private int yawInputHash = Animator.StringToHash("Yaw Input");
        private int speedHash = Animator.StringToHash("Speed");
        private int isSprintingHash = Animator.StringToHash("IsSprinting");
        private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        protected AnimationCurve locomotionCurve;
        public bool IsTurning { get; set; }
        public bool IsSprinting { get; private set; } = false;

        public LocomotionState(MovementStateDriver ctx, float accel, float decel, MotionType motionType) : base(ctx)
        {
            Acceleration = accel;
            Deceleration = decel;
            MotionType = motionType;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (Character.IsUnderPlayerControl)
                if (Character.SprintPressed && MovementInput.y > 0.5f)
                {
                    IsSprinting = true;
                    Animator.SetBool(isSprintingHash, true);
                    InternalSpeedMultiplier = sprintSpeed;
                    if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
                    {
                        CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 90f, Time.deltaTime);
                    }
                }
                else
                {
                    Animator.SetBool(isSprintingHash, false);
                    IsSprinting = false;
                    InternalSpeedMultiplier = runSpeed;
                    if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
                    {
                        CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 60f, Time.deltaTime);
                    }
                }
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();
            float animatedspeed = 0;
            //animatedspeed = locomotionCurve.Evaluate(Character.TotalSpeedMultiplier);
            animatedspeed = Character.TotalSpeedMultiplier;
            isMoving = MovementInput.magnitude > 0;
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


        public override void Enter()
        {
            base.Enter();
            InternalSpeedMultiplier = runSpeed;
        }
        public override void Exit()
        {
            base.Exit();
            Animator.SetFloat(speedHash, 0);
            Animator.SetFloat(verticalSpeedHash, 0);
            Animator.SetFloat(horizontalSpeedHash, 0);
            Animator.SetBool(isSprintingHash, false);
        }

    }
    public abstract class AerialState : BaseState
    {
        protected AerialState(MovementStateDriver ctx) : base(ctx)
        {
        }

        protected override void ChangeVelocity()
        {

            Vector3 desiredVelocity = moveDirection * Character.CurrentSpeed;
            if (MovementInput.magnitude > 0)
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(
                Character.HorizontalVelocity,
                desiredVelocity,
                Time.deltaTime * Acceleration);
            }
            else
            {
                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);

                Character.HorizontalVelocity = Vector3.Lerp(
                Character.HorizontalVelocity,
                Vector3.zero,
                Time.deltaTime * Character.Deceleration);
            }

        }
    }
    public abstract class GroundedState : BaseState
    {
        protected GroundedState(MovementStateDriver ctx) : base(ctx)
        {
        }
        protected override void ChangeVelocity()
        {
            base.ChangeVelocity();
            if (MovementInput.magnitude > 0)
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, InternalSpeedMultiplier * MovementInput.magnitude, Character.Acceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, moveDirection * Character.CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {

                Character.InternalSpeedMultiplier = Mathf.Lerp(Character.InternalSpeedMultiplier, 0, Character.Deceleration * Time.deltaTime);
                Character.HorizontalVelocity = Vector3.Lerp(Character.HorizontalVelocity, Vector3.zero, Character.Deceleration * Time.deltaTime);
            }
        }
    }
    public class FallingState : AerialState
    {
        public FallingState(MovementStateDriver ctx, float accel, float decel, float floatspd, MotionType motionType) : base(ctx)
        {
            Acceleration = accel;
            Deceleration = decel;
            MotionType = motionType;
            floatingSpeed = floatspd;
            InitializeTimers();
        }

        float floatingSpeed;

        int landingStateHash = Animator.StringToHash("LandingState");
        int isFallingHash = Animator.StringToHash("IsFalling");

        private float landingDuration = 0.1f;
        private float minFallingTimeToLandingTransition = 0.8f;
        private float fallingTimeForHardLanding = 1f;
        private float hardLandingDuration = 1.5f;
        private bool autoCalculateLandingDuration = false;



        // State Management
        private CountdownTimer landingCoolDownTimer;
        private StopwatchTimer fallingTimer;
        public bool IsLanding { get; private set; }
        public float FallingTime => fallingTimer.CurrentTime;

        private void InitializeTimers()
        {
            landingCoolDownTimer = new CountdownTimer(landingDuration);
            fallingTimer = new StopwatchTimer();

        }

        public void StartFallingTimer() => fallingTimer.Start();

        public void StopFallingTimer()
        {
            fallingTimer.Stop();

            landingCoolDownTimer.Reset(landingDuration);
        }

        public void OnLanding()
        {
            landingCoolDownTimer.Start();
            IsLanding = true;

            SetLandingAnimationState();
        }

        private void SetLandingAnimationState()
        {
            Animator.SetInteger(landingStateHash, FallingTime <= fallingTimeForHardLanding
                ? 1 : 2);
        }

        public override void Enter()
        {
            base.Enter();
            InternalSpeedMultiplier = floatingSpeed;
            StartFallingTimer();
            Animator?.SetBool(isFallingHash, true);
            Animator?.SetInteger(landingStateHash, 0);
        }
        public override void Exit()
        {
            base.Exit();
            Animator?.SetBool(isFallingHash, false);
            Animator?.SetInteger(landingStateHash, 0);
            StopFallingTimer();

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (landingCoolDownTimer.IsFinished)
            {
                IsLanding = false;

            }
        }
    }
    public class JumpState : BaseState
    {
        CharacterGravityModule characterGravityModule;
        public JumpState(MovementStateDriver ctx, float vertforce, float planforce, MotionType motionType) : base(ctx)
        {
            characterGravityModule = ctx.GetComponent<CharacterGravityModule>();
            MotionType = motionType;
            jumpVerticalImpulse = vertforce;
            jumpPlanarImpulse = planforce;
        }

        private float jumpVerticalImpulse;
        private float jumpPlanarImpulse;

        private void ExecuteJump()
        {
            switch (MotionType)
            {
                case MotionType.CharacterController:
                    {
                        characterGravityModule.VerticalVelocity = Mathf.Sqrt(jumpVerticalImpulse * -2f * Physics.gravity.y);
                        if (MovementInput.magnitude > 0)
                        {
                            Vector3 movement = new Vector3(MovementInput.x, 0, MovementInput.y);

                            Vector3 cam = Camera.main.transform.forward;

                            Character.HorizontalVelocity += Quaternion.LookRotation(new Vector3(cam.x, 0, cam.z)) * movement * Character.TotalSpeedMultiplier * jumpPlanarImpulse;
                        }
                    }
                    break;
                default:
                    break;
            }
            Animator?.CrossFade("JumpUpward", 0.05f);
            Character.JumpPressed = false;

        }

        public override void Enter()
        {
            base.Enter();
            ExecuteJump();
        }
    }
    public class CrouchState : LocomotionState
    {
        public CrouchState(MovementStateDriver ctx, float accel, float decel, MotionType motionType) : base(ctx, accel, decel, motionType)
        {

            Acceleration = accel;
            Deceleration = decel;
            MotionType = motionType;
            characterController = ctx.GetComponent<CharacterController>();
            InitializeHeightValues();
        }
        float crouchSpeed = 0.3f;

        private float crouchHeightMultiplier = 0.5f;
        private CharacterController characterController;
        public float CrouchHeight { get; private set; }
        public float StandingHeight { get; private set; }

        private void InitializeHeightValues()
        {
            StandingHeight = characterController.height;
            CrouchHeight = StandingHeight * crouchHeightMultiplier;
        }
        public override void LogicUpdate()
        {

        }
        public override void Enter()
        {
            base.Enter();
            Character.Height = CrouchHeight;
            InternalSpeedMultiplier = crouchSpeed;
        }
        public override void Exit()
        {
            base.Exit();
            Character.Height = StandingHeight;
        }
    }
    public class RollState : BaseState
    {
        public RollState(MovementStateDriver ctx, MotionType motionType) : base(ctx)
        {
            MotionType = motionType;
        }
        public bool IsRolling { get => Animator.GetBool(isRollingHash); private set => Animator.SetBool(isRollingHash, value); }
        int isRollingHash = Animator.StringToHash("IsRolling");
        int RollHash = Animator.StringToHash("Roll");

        private void FixedUpdate()
        {
            if (Character.EvadePressed && Sensors.IsGrounded && !IsRolling)
            {
                IsRolling = true;
            }
        }

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
            Character.EvadePressed = false;
            IsRolling = true;
            base.Enter();
        }
    }
    public class WallrunState : GroundedState
    {
        public WallrunState(MovementStateDriver ctx, float accel, float decel, MotionType motionType) : base(ctx)
        {
            Acceleration = accel;
            Deceleration = decel;
            MotionType = motionType;
        }
        private float wallRunGravityMultiplier = 0f;
        private float wallRunSpeed = 1;
        private bool useWallCliping = false;
        Vector3 normal;
        Vector3 magnit => -normal;
        public bool IsWallrunning { get; private set; }
        //private void OnJump()
        //{
        //    if (!wallJumpCooldownTimer.IsRunning)
        //    {
        //        if ((Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight) && !Sensors.IsGrounded && MovementInput.y > 0)
        //        {
        //            IsJumping = true;
        //            wallJumpCooldownTimer.Start();
        //            Character.ExitingWallTimer.Start();
        //        }
        //    }
        //}
        //TODO Transfer to WalljumpState class

        //public void WallJump()
        //{
        //    Vector3 wallNormal;
        //    if (Sensors.IsObstacleLegsRight)
        //    {
        //        wallNormal = Sensors.LegsRightHit.normal;
        //    }
        //    else
        //    {
        //        wallNormal = Sensors.LegsLeftHit.normal;
        //    }

        //    CharacterGravity.VerticalVelocity = Mathf.Sqrt(jumpForce * -2 * Physics.gravity.y);
        //    Character.HorizontalVelocity = wallNormal * wallJumpForce + Character.transform.forward * Character.CurrentSpeed;

        //    Character.RotateToDirection(Character.HorizontalVelocity);
        //}
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

            Vector3 wallAlong = Vector3.Cross(normal, Character.transform.up);
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
            if (CharacterGravity.VerticalVelocity <= 0)
            {
                CharacterGravity.VerticalVelocity *= wallRunGravityMultiplier;

            }
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(moveDirection, TurnSmoothTime);
        }
        public override void Enter()
        {
            base.Enter();
            IsWallrunning = true;
            InternalSpeedMultiplier = wallRunSpeed;
        }
        public override void Exit()
        {
            base.Exit();
            IsWallrunning = false;
        }
    }
}
