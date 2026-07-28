using LOGIYGames.Movement;
using LOGIYGames.Shared.Data;
using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public partial class Character : MonoModuleBase, IControllable, IDamageable
    {
        public CharacterInput Input { get; private set; }
        public IMovementStrategy MovementStrategy { get; set; }
        public IRotationStrategy RotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }
        public IMovementStrategy DefaultMovementStrategy { get; set; }

        public IEventDispatcher EventBus { get; private set; } = new EventDispatcher();

        [Header("References")]

        [field: SerializeField] public CameraTarget CameraTarget { get; private set; }
        [field: SerializeField] public MovementWrapperBase Motor { get; private set; }
        [field: SerializeField] public SensorsModule Sensors { get; private set; }
        public MovementRuntime RuntimeMovement { get; private set; }

        public CharacterStats Stats { get; private set; }

        [field:SerializeField] public EffectsController EffectSystem {  get; private set; }

        #region Modules
        public TargetingController TargetingController { get; private set; }
        public HealthController HealthController { get; private set; }
        public StaminaController StaminaController { get; private set; }
        public JumpController JumpController { get; private set; }
        public MantlingController MantlingController { get; private set; }
        #endregion

        [Header("State Machine Configuration")]
        public StateMachine MovementStateMachine { get; private set; }
        public MovementBuilder movementPreset;
        private Dictionary<Type, CharacterMovementState> m_movementStates = new();
        public bool IsGrounded { get => Sensors.IsGrounded; }

        public float Radius { get => Motor.Radius; set => Motor.Radius = value; }
        public float MaxStepHeight { get => Motor.MaxStepHeight; set => Motor.MaxStepHeight = value; }
        [field: SerializeField] public float Height { get; set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;
        private void Awake()
        {
            RuntimeMovement = new();
            Stats = new();
            Stats.SetBase(StatType.BaseHealth, 100);
            Stats.SetBase(StatType.BaseStamina, 50);
            Stats.SetBase(StatType.BaseMana, 10);
            Stats.SetBase(StatType.Vitality, 10);
            Stats.SetBase(StatType.Intelegence, 1);
            Stats.SetBase(StatType.AttackBase, 1);
            Stats.SetBase(StatType.DefenseBase, 1);
            Stats.SetBase(StatType.CritRate, 15);
            Stats.SetBase(StatType.CritDamage, 50);

            HealthController = new HealthController(Stats);
            StaminaController = new StaminaController(Stats, 1);
            MantlingController = GetComponent<MantlingController>();
            InitializeStateMachine();
            TargetingController = new();
            JumpController = new(this);
            EffectSystem = new(this);
        }
        private void Start()
        {
            Motor.Height = Height;
            Motor.Center = new Vector3(0, Height / 2.0f, 0);
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
            RuntimeMovement.TargetRotation = RotationStrategy.GetRotation();
            RuntimeMovement.TargetDirection = MovementStrategy.GetMovementDirection();
            UpdateVelocity();
            CalculateDeltaYaw();
            MovementStateMachine.Update();
            StaminaController.Tick();
            EffectSystem.Update();
        }
        private void SmoothHeightChanging()
        {
            if (Height == Motor.Height && Motor.Center.y == Height) return;
            if (!Mathf.Approximately(Motor.Height, Height) || !Mathf.Approximately(Motor.Center.y, Height / 2.0f))
            {
                Motor.Height = Mathf.Lerp(Motor.Height, Height, HeightChangingSmoothTime * Time.deltaTime);
                Motor.Center = Vector3.Lerp(Motor.Center, new Vector3(0, Height / 2.0f, 0), HeightChangingSmoothTime * Time.deltaTime);
            }
            else
            {
                Motor.Height = Height;
                Motor.Center = new Vector3(0, Height / 2.0f, 0);
            }
        }

        #region Rotation Methods
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Motor.transform.up);
            Rotate(targetRotation, turnSmoothTime);
        }
        public void RotateToPosition(Vector3 position, float turnSmoothTime = 0)
        {
            Vector3 desiredDirection = position - Motor.transform.position;
            RotateToDirection(desiredDirection.normalized, turnSmoothTime);
        }

        public void Rotate(Quaternion targetRotation, float turnSpeed = 0)
        {
            if (turnSpeed > 0f)
            {
                Quaternion smoothedRotation = Quaternion.Slerp(Motor.Rotation, targetRotation, turnSpeed * Time.deltaTime);
                Motor.SetRotation(smoothedRotation);
            }
            else
            {
                Motor.SetRotation(targetRotation);
            }
        }

        private void CalculateDeltaYaw()
        {
            RuntimeMovement.DeltaYaw = Mathf.DeltaAngle(transform.eulerAngles.y, RuntimeMovement.TargetRotation.eulerAngles.y);
            if (Mathf.Abs(RuntimeMovement.DeltaYaw) < 0.01f)
            {
                RuntimeMovement.DeltaYaw = 0;
            }
        }

        #endregion
        #region Movement Methods

        public void Move()
        {
            Motor.Move(RuntimeMovement.TargetVelocity);
        }

        private void UpdateVelocity()
        {
            if (Input.MovementInput.magnitude > 0)
            {

                RuntimeMovement.TargetVelocity = Vector3.Lerp(RuntimeMovement.TargetVelocity, RuntimeMovement.TargetDirection.normalized * RuntimeMovement.CurrentSpeed, RuntimeMovement.AccelerationData.Acceleration * Time.deltaTime);
            }
            else
            {
                RuntimeMovement.TargetVelocity = Vector3.Lerp(RuntimeMovement.TargetVelocity, Vector3.zero, RuntimeMovement.AccelerationData.Deceleration * Time.deltaTime);
            }
        }

        public void ForceMove(Vector3 moveDirection)
        {
            Motor.ForceMove(moveDirection);
        }
        public void SetPosition(Vector3 position)
        {
            Motor.SetPosition(position);
        }
        public void Jump(Vector3 jumpForce)
        {
            Motor.AddForce(jumpForce);
        }

        public void ResetVelocity()
        {
            RuntimeMovement.TargetVelocity = Vector3.zero;
            RuntimeMovement.TargetDirection = Vector3.zero;
            Motor.ResetVelocity();
        }
        public void ResetSpeed() => RuntimeMovement.AccelerationData = new();
        #endregion
        #region Movement State Machine
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
            if (RuntimeMovement.TargetVelocity.magnitude > 0)
            {
                localDir = Motor.transform.InverseTransformDirection(RuntimeMovement.TargetVelocity);
            }
            else
            {
                localDir = Motor.transform.InverseTransformDirection(Motor.Velocity);
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

        public void TakeDamage(DamageData damage)
        {
            HealthController.TakeDamage(damage);
        }

    }
}
