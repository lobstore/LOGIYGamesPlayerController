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
        protected float TurnSmoothingTime = 10f;
        protected bool moveBeforeRotation;
        protected float InternalSpeedMultiplier = 0f;
        protected Character Character;
        protected SensorsModule Sensors;
        protected Animator Animator;
        protected CharacterController CController;
        protected CharacterGravityModule CharacterGravity;

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
                Character.Rotate(targetRotation, TurnSmoothingTime);
            }
        }

        private void RotateAlongCamera()
        {
            var targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            Character.Rotate(targetRotation, TurnSmoothingTime);
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
        protected float walkSpeed;
        protected float runSpeed;
        protected float sprintSpeed;
        private int isMovingHash = Animator.StringToHash("IsMoving");
        private int yawInputHash = Animator.StringToHash("Yaw Input");
        private int speedHash = Animator.StringToHash("Speed");
        private int isSprintingHash = Animator.StringToHash("IsSprinting");
        private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        protected AnimationCurve locomotionCurve;
        public bool IsTurning { get; set; }
        public bool IsSprinting { get; private set; } = false;

        public LocomotionState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            walkSpeed = statesDataSO.walkSpeed;
            runSpeed = statesDataSO.runSpeed;
            sprintSpeed = statesDataSO.sprintSpeed;
            Acceleration = statesDataSO.locomotonAcceleration;
            Deceleration = statesDataSO.locomotonDeceleration;
            MotionType = statesDataSO.locomotionMotionType;
            locomotionCurve = statesDataSO.locomotionCurve;
            TurnSmoothingTime = statesDataSO.turnSmoothingTimeLocomotion;
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
        private bool useAutoCalculatedPlayerSpeedMultiplier = true;

        protected float slopeAffectRate = 0.2f;
        Vector3 projectedVelocity;
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
            if (useAutoCalculatedPlayerSpeedMultiplier)
            {
                CalculateSlopeSpeedMultiplier();
            }
        }
        private void CalculateSlopeSpeedMultiplier()
        {
            projectedVelocity = Vector3.ProjectOnPlane(
            Vector3.down,
            Sensors.BelowHit.normal
            );
            // Вычисляем косинус угла между направлением движения и направлением склона
            float dot = Vector3.Dot(moveDirection, projectedVelocity);

            // Теперь множитель скорости зависит от направления движения:
            // - dot > 0: движение вниз по склону — ускорение
            // - dot < 0: движение в гору — замедление
            // - dot ≈ 0: движение перпендикулярно склону — без изменений


            // Итоговый множитель скорости:
            var targetMultiplier = Mathf.Clamp(1f + dot * slopeAffectRate, 0.5f, 1.5f);
            Character.ExternalSpeedMultiplier = Mathf.Lerp(
            Character.ExternalSpeedMultiplier,
            targetMultiplier,
            Time.deltaTime * Character.Acceleration);
        }
    }
    public class FallingState : AerialState
    {
        public FallingState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            Acceleration = statesDataSO.airAcceleration;
            Deceleration = statesDataSO.airDeceleration;
            MotionType = statesDataSO.fallingMotionType;
            floatingSpeed = statesDataSO.airSpeed;
            TurnSmoothingTime = statesDataSO.turnSmoothingTimeFalling;
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
        public JumpState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            characterGravityModule = ctx.GetComponent<CharacterGravityModule>();
            MotionType = statesDataSO.jumpMotionType;
            jumpVerticalImpulse = statesDataSO.verticalJumpForce;
            jumpPlanarImpulse = statesDataSO.planarJumpForce;
        }

        private float jumpVerticalImpulse;
        private float jumpPlanarImpulse;

        private void Jump()
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
            Jump();
        }
    }
    public class CrouchState : LocomotionState
    {
        public CrouchState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx, statesDataSO)
        {

            Acceleration = statesDataSO.crouchAcceleration;
            Deceleration = statesDataSO.crouchDeceleration;
            MotionType = statesDataSO.crouchMotionType;
            crouchSpeed = statesDataSO.crouchSpeed;
            TurnSmoothingTime = statesDataSO.turnSmoothingTimeCrouch;
            characterController = ctx.GetComponent<CharacterController>();
            InitializeHeightValues();
        }
        float crouchSpeed;

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
        public RollState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            MotionType = statesDataSO.rollMotionType;
            rollForce = statesDataSO.rollJumpForce;
        }
        public bool IsRolling { get => Animator.GetBool(isRollingHash); private set => Animator.SetBool(isRollingHash, value); }
        int isRollingHash = Animator.StringToHash("IsRolling");
        int RollHash = Animator.StringToHash("Roll");
        float rollForce;
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
        public WallrunState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            Acceleration = statesDataSO.wallrunAcceleration;
            Deceleration = statesDataSO.wallrunDeceleration;
            MotionType = statesDataSO.walljumpMotionType;
            wallRunSpeed = statesDataSO.wallrunSpeed;
            wallRunGravityMultiplier = statesDataSO.wallrunGravityMultiplier;
            useWallCliping = statesDataSO.useWallclippingWallrun;
            MotionType = statesDataSO.wallrunMotionType;
        }
        private float wallRunGravityMultiplier;
        private float wallRunSpeed;
        private bool useWallCliping;
        Vector3 normal;
        Vector3 magnit => -normal;
        public bool IsWallrunning { get; private set; }

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
            if (CharacterGravity.VerticalVelocity <= 0)
            {
                CharacterGravity.VerticalVelocity *= wallRunGravityMultiplier;

            }
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(moveDirection, TurnSmoothingTime);
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
    public class ClimbState : GroundedState
    {
        public ClimbState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            Acceleration = statesDataSO.climbSpeed;
            Deceleration = statesDataSO.climbDeceleration;
            MotionType = statesDataSO.climbMotionType;
            wallClimbSpeed = statesDataSO.climbSpeed;

            useWallCliping = statesDataSO.useWallclippingClimb;
        }
        private float wallClimbSpeed;
        private bool useWallCliping;
        public bool IsClimbing { get; private set; }
        Vector3 normal;
        Vector3 magnit => -normal;
        private void Magnit()
        {
            CController.Move(magnit * Time.deltaTime);

        }
        protected override void GetMovementDirection()
        {

            normal = Sensors.LegsFrontHit.normal;

            Vector3 wallAlongUp = Vector3.ProjectOnPlane(Character.transform.up, normal).normalized;

            moveDirection = wallAlongUp;
        }
        protected override void Move()
        {
            base.Move();
            if (useWallCliping)
            {
                Magnit();

            }
        }
        protected override void ChangeVelocity()
        {
            base.ChangeVelocity();
            if (CharacterGravity.VerticalVelocity <= 0)
            {
                CharacterGravity.VerticalVelocity = 0;

            }
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(-Sensors.LegsFrontHit.normal, 0);
        }
        public override void Enter()
        {
            base.Enter();
            IsClimbing = true;
            InternalSpeedMultiplier = wallClimbSpeed;
            Character.InternalSpeedMultiplier = InternalSpeedMultiplier;
        }
        public override void Exit()
        {
            base.Exit();
            IsClimbing = false;
        }
        public bool CanClimbWall()
        {
            return Sensors.IsObstacleLegsFront
                        && MovementInput.y > 0;
            //&& Vector3.Angle(Character.transform.forward, -Sensors.LegsFrontHit.normal) < 60
            //&& Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 30;
        }
    }
    public class WallJumpState : BaseState
    {
        public WallJumpState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            verticalImpulse = statesDataSO.verticalWallrunJumpForce;
            planarImpulse = statesDataSO.planarWallrunJumpForce;
            MotionType = statesDataSO.walljumpMotionType;
        }
        bool IsWallJumpBackward;
        float verticalImpulse;
        float planarImpulse;
        public void WallJump()
        {
            Vector3 wallNormal;

            if (Sensors.IsObstacleLegsFront)
            {
                CharacterGravity.VerticalVelocity = Mathf.Sqrt(verticalImpulse * -2 * Physics.gravity.y);
                Character.HorizontalVelocity = Sensors.LegsFrontHit.normal * planarImpulse;
                IsWallJumpBackward = true;
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
                CharacterGravity.VerticalVelocity = Mathf.Sqrt(verticalImpulse * -2 * Physics.gravity.y);
                Character.HorizontalVelocity = wallNormal * planarImpulse + Character.transform.forward * planarImpulse;
            }





        }
        protected override void Rotate()
        {
            if (!IsWallJumpBackward)
            {
                //Character.RotateToDirection(Character.HorizontalVelocity);

            }
            else
            {
                Character.RotateToDirection(Sensors.LegsFrontHit.normal);
            }
        }
        public override void Enter()
        {
            base.Enter();
            WallJump();
        }
    }
    public class SlideState : GroundedState
    {
        private float requiredSpeedMultiplierToSlip;
        private float SlideSlopeAngleLimit;
        int isSlidingHash = Animator.StringToHash("IsSliding");
        public SlideState(MovementStateDriver ctx, StatesDataSO statesDataSO) : base(ctx)
        {
            slideSpeed = statesDataSO.slideSpeed;

        }
        float slideSpeed;
        public bool IsSliding { get; set; }
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
            Character.HorizontalVelocity += projectedVelocity.normalized * Time.deltaTime * slideSpeed;
        }
        protected override void Rotate()
        {
            Character.RotateToDirection(Character.HorizontalVelocity, 8);
        }
        public override void Enter()
        {
            IsSliding = true;
            Animator.SetBool(isSlidingHash, IsSliding);
            Character.HorizontalVelocity = Character.HorizontalVelocity / 2;
            base.Enter();
        }
        public override void Exit()
        {
            IsSliding = false;
            Animator.SetBool(isSlidingHash, IsSliding);
            base.Exit();
        }
    }
}
