using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames.CharacterCore
{
    [RequireComponent(typeof(CameraTargetModule))]
    public class CharacterModule : MonoModuleBase, IControllable
    {
        public CharacterInput Input { get; private set; }
        public IMovementStrategy MovementStrategy { get; set; }
        public IRotationStrategy RotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }
        public IMovementStrategy DefaultMovementStrategy { get; set; }
        public IEventDispatcher EventBus { get; private set; }

        [Header("References")]


        [field: SerializeField] private ControllerWrapperBase m_motor;
        [field: SerializeField] public SensorsModule Sensors { get; private set; }

        public int JumpCount;

        #region Modules
        public CharacterTargetingModule Targeting { get; private set; }
        public ComboController ComboController { get; private set; }
        public WeaponController WeaponController { get; private set; }
        public AbilityController AbilityController { get; private set; }
        #endregion
        #region State Machine Configuration
        [Header("State Machine Configuration")]
        public StateMachineDebugModule MovementStateMachineDebugModule { get; private set; }
        public MovementBuilder movementPreset;
        private Dictionary<Type, CharacterMovementState> m_movementStates = new();
        public StateMachine MovementStateMachine { get; private set; }
        #endregion
        #region Runtime States
        public bool IsFalling { get; set; }
        public bool IsFlying { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsGrounded { get => Sensors.IsGrounded; }
        public bool IsSliding { get; set; }
        public bool IsOnLadder { get; set; }
        public bool IsWallClimbing { get; set; }
        public bool IsWallRunning { get; set; }
        public bool IsSwimming { get; set; }
        public bool IsAimig { get; set; }
        public bool IsMantling { get; set; }
        #endregion
        #region Velocity Variables
        [Header("Movement Configuration")]
        public CharacterVelocityData VelocityData { get; private set; }
        public float BaseSpeed;
        public AccelerationData AccelerationData { get; set; }
        public float Speed{ get; set; }
        public float CurrentSpeed => Speed * BaseSpeed;
        public float TurnSmoothTime { get; set; } = 5f;
        public Quaternion TargetRotation { get; set; }
        public Vector3 TargetDirection { get; set; }

        #endregion
        #region Collider Properties
        public float Radius { get => m_motor.Radius; set => m_motor.Radius = value; }
        public float MaxStepHeight { get => m_motor.MaxStepHeight; set => m_motor.MaxStepHeight = value; }
        [field: SerializeField] public float Height { get; set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;

        #endregion

        #region Camera References
        public CameraTargetModule CameraTarget { get; set; }
        public Transform CameraLookAt => CameraTarget.CameraLookAt;
        public Transform CameraFollow => CameraTarget.CameraFollow;

        #endregion
        public float DeltaYaw { get; set; }

        public UnityEvent OnControlReleased { get; } = new();


        private void Awake()
        {

            EventBus = new EventDispatcher();
            CameraTarget = GetComponent<CameraTargetModule>();
            WeaponController = GetComponent<WeaponController>();
            ComboController = GetComponent<ComboController>();
            AbilityController = GetComponent<AbilityController>();


            Targeting = new();
            VelocityData = new();
            AccelerationData = new();

            EventsSubscription();
        }
        private void Start()
        {
            m_motor.Height = Height;
            m_motor.Center = new Vector3(0, Height / 2.0f, 0);
            InitializeStateMachine();
            MovementStateMachineDebugModule = new StateMachineDebugModule(MovementStateMachine);
        }
        private void EventsSubscription()
        {
            EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        Jump(TargetDirection * evt.planarForce, transform.up * evt.verticalForce);
                        JumpCount++;
                        GetComponent<StaminaModule>().TryUse(20);
                        break;
                    case JumpType.HangJump:
                        Jump(Sensors.LegsFrontHit.normal * evt.planarForce, evt.verticalForce * transform.up);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    case JumpType.Roll:
                        Jump(transform.forward * evt.planarForce, transform.up * evt.verticalForce);
                        break;
                    case JumpType.Dash:
                        Jump(TargetDirection * evt.planarForce, transform.up * evt.verticalForce);
                        break;
                    case JumpType.Slip:
                        Jump(TargetDirection * evt.planarForce, Vector3.zero);
                        break;
                    default:
                        break;
                }
            });
        }
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            MovementStateMachine.FixedUpdate();
        }
        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            MovementStateMachine.LateUpdate();
            SmoothHeightChanging();
        }
        public override void OnUpdate(float deltaTime)
        {

            base.OnUpdate(deltaTime);
            UpdateVelocity();
            TargetRotation = RotationStrategy.GetRotation();
            CalculateDeltaYaw();
            TargetDirection = MovementStrategy.GetMovementDirection();
            MovementStateMachine.Update();
            MovementStateMachineDebugModule.Update();

        }

        private void SmoothHeightChanging()
        {
            if (Height == m_motor.Height && m_motor.Center.y == Height) return;
            if (!Mathf.Approximately(m_motor.Height, Height) || !Mathf.Approximately(m_motor.Center.y, Height / 2.0f))
            {
                m_motor.Height = Mathf.Lerp(m_motor.Height, Height, HeightChangingSmoothTime * Time.deltaTime);
                m_motor.Center = Vector3.Lerp(m_motor.Center, new Vector3(0, Height / 2.0f, 0), HeightChangingSmoothTime * Time.deltaTime);
            }
            else
            {
                m_motor.Height = Height;
                m_motor.Center = new Vector3(0, Height / 2.0f, 0);
            }
        }

        #region Rotation Methods
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, m_motor.transform.up);
            Rotate(targetRotation, turnSmoothTime);
        }
        public void RotateToPosition(Vector3 position, float turnSmoothTime = 0)
        {
            Vector3 desiredDirection = position - m_motor.transform.position;
            RotateToDirection(desiredDirection.normalized, turnSmoothTime);
        }

        public void Rotate(Quaternion targetRotation, float turnSpeed = 0)
        {
            if (turnSpeed > 0f)
            {
                Quaternion smoothedRotation = Quaternion.Slerp(m_motor.Rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
                m_motor.SetRotation(smoothedRotation);
            }
            else
            {
                m_motor.SetRotation(targetRotation);
            }
        }

        private void CalculateDeltaYaw()
        {
            DeltaYaw = Mathf.DeltaAngle(transform.eulerAngles.y, TargetRotation.eulerAngles.y);
            if (Mathf.Abs(DeltaYaw) < 0.01f)
            {
                DeltaYaw = 0;
            }
        }

        #endregion
        #region Movement Methods

        public void Move()
        {
            m_motor.Move(VelocityData.Locomotion);
        }

        private void UpdateVelocity()
        {
            if (Input.MovementInput.magnitude > 0)
            {

                VelocityData.Locomotion = Vector3.Lerp(VelocityData.Locomotion, TargetDirection.normalized * CurrentSpeed, AccelerationData.Acceleration * Time.deltaTime);
            }
            else
            {
                VelocityData.Locomotion = Vector3.Lerp(VelocityData.Locomotion, Vector3.zero, AccelerationData.Deceleration * Time.deltaTime);
            }
        }

        public void ForceMove(Vector3 moveDirection)
        {
            m_motor.ForceMove(moveDirection);
        }
        public void SetPosition(Vector3 position)
        {
            m_motor.SetPosition(position);
        }
        public void Jump(Vector3 hForce, Vector3 vForce)
        {
            VelocityData.Locomotion = hForce;
            VelocityData.Gravity = vForce;
            m_motor.AddForce(new Vector3(VelocityData.Locomotion.x, VelocityData.Gravity.y, VelocityData.Locomotion.z));

        }
        private Vector3 ProjectVelocity()
        {
            return Vector3.ProjectOnPlane(VelocityData.Locomotion, Sensors.BelowHit.normal) + Vector3.ProjectOnPlane(-m_motor.transform.up * Speed, Sensors.BelowHit.normal);
        }
        public void Slide()
        {
            m_motor.Move(ProjectVelocity());
        }

        public void ResetVelocity()
        {
            VelocityData.Reset();
            TargetDirection = Vector3.zero;
            m_motor.ResetVelocity();
        }
        public void ResetSpeed() => AccelerationData = new();
        #endregion
        private void InitializeStateMachine()
        {
            MovementStateMachine = new StateMachine();
            if (movementPreset != null)
            {
                movementPreset.Build(this);

            }
            else
            {
                Debug.LogError("No MovementPreset provided");
            }
        }
        #region Movement State Machine

        public void AddMovementState(CharacterMovementState state)
        {
            m_movementStates[state.GetType()] = state;

            MovementStateMachine.AddState(state);
        }
        public void RemoveMovementState<T>() where T : CharacterMovementState
        {
            m_movementStates.Remove(typeof(T));

            MovementStateMachine.RemoveState<T>();
        }
        public T GetMovementState<T>() where T : CharacterMovementState
        {
            if (m_movementStates.TryGetValue(typeof(T), out var state))
                return state as T;

            return null;
        }

        public bool HasMovementState<T>() where T : CharacterMovementState
        {
            return m_movementStates.ContainsKey(typeof(T));
        }
        #endregion
        #region Action State Machine
        #endregion

        #region IControllable
        public void UpdateInput(CharacterInput inputReader)
        {
            Input = inputReader;
        }
        public void ResetInput()
        {
            Input = new CharacterInput();
        }
        public void ResetStrategies()
        {
            RotationStrategy = DefaultRotationStrategy;
            MovementStrategy = DefaultMovementStrategy;
        }

        #endregion
        public Direction GetRelativeMovementDirection()
        {
            Vector3 localDir;
            if (VelocityData.Locomotion.magnitude > 0)
            {
                localDir = m_motor.transform.InverseTransformDirection(VelocityData.Locomotion);
            }
            else
            {
                localDir = m_motor.transform.InverseTransformDirection(m_motor.Velocity);
            }
            float forwardDot = Vector3.Dot(localDir, Vector3.forward);
            float rightDot = Vector3.Dot(localDir, Vector3.right);
            float upDot = Vector3.Dot(localDir, Vector3.up);
            Direction direction;
            if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
            {
                if (forwardDot > 0)
                    direction = Direction.Forward;
                else
                    direction = Direction.Backward;
            }
            else if (Mathf.Abs(forwardDot) < Mathf.Abs(rightDot))
            {
                if (rightDot > 0)
                    direction = Direction.Right;
                else
                    direction = Direction.Left;
            }
            else
            {
                direction = Direction.NoMovement;
            }

            return direction;
        }

    }
}
