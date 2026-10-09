using Alchemy.Inspector;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.TextCore.Text;
namespace LOGIYGames.CharacterCore
{
    public partial class Actor : MonoModuleBase, IControllable, IDamageable
    {
        public CharacterInput Input { get; private set; }
        public IMovementStrategy MovementStrategy { get; set; }
        public IRotationStrategy RotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }
        public IMovementStrategy DefaultMovementStrategy { get; set; }

        public IEventDispatcher EventBus { get; private set; } = new EventDispatcher();

        [field: SerializeField] public CameraTarget CameraTarget { get; private set; }
        [field: SerializeField] public CameraTarget ActionCameraTarget { get; private set; }
        [field: SerializeField] public MovementWrapperBase Motor { get; private set; }
        [field: SerializeField] public SensorsModule Sensors { get; private set; }
        [ReadOnly] public MovementRuntimeData RuntimeMovement;
        [ReadOnly] public EffectsController Effects;
        public StatsController Stats;


        #region Modules
        public TargetingController TargetingController;
        public HealthController HealthController { get; private set; }
        public StaminaController StaminaController { get; private set; }
        #endregion

        [Header("State Machine Configuration")]
        public StateMachine MovementStateMachine { get; private set; }
        public MovementBuilder movementPreset;
        private Dictionary<Type, MovementStateBase> m_movementStates = new();
        public bool IsGrounded { get => Sensors.IsGrounded; }

        private void Awake()
        {
            //Stats = new();
            //Stats.SetBase(StatType.BaseHealth, 100);
            //Stats.SetBase(StatType.BaseStamina, 50);
            //Stats.SetBase(StatType.BaseMana, 10);
            //Stats.SetBase(StatType.Vitality, 10);
            //Stats.SetBase(StatType.Intelegence, 1);
            //Stats.SetBase(StatType.AttackBase, 1);
            //Stats.SetBase(StatType.DefenseBase, 1);
            //Stats.SetBase(StatType.CritRate, 15);
            //Stats.SetBase(StatType.CritDamage, 50);
            Motor = GetComponent<MovementWrapperBase>();
            Sensors = GetComponent<SensorsModule>();
            HealthController = new HealthController(Stats);
            StaminaController = new StaminaController(Stats, 1);
            InitializeStateMachine();
            Effects = new(this);
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
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (MovementStrategy != null)
            {
                RuntimeMovement.TargetDirection = MovementStrategy.GetMovementDirection();
            }
            if (RotationStrategy != null)
            {
                RuntimeMovement.TargetRotation = RotationStrategy.GetRotation();

            }
            UpdateSpeed();
            CalculateDeltaYaw();
            MovementStateMachine.Update();
            StaminaController.Tick();
            HealthController.Tick();
            Effects.Update();
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
        public void Rotate(Quaternion targetRotation)
        {
            Rotate(targetRotation, RuntimeMovement.TurnSmoothTime);
        }
        public void Rotate()
        {
            Rotate(RuntimeMovement.TargetRotation, RuntimeMovement.TurnSmoothTime);
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
        private void UpdateSpeed()
        {
            if (RuntimeMovement.CurrentSpeed < RuntimeMovement.TargetSpeed && Input.MovementInput.magnitude > 0)
            {

                RuntimeMovement.CurrentSpeed = Mathf.MoveTowards(RuntimeMovement.CurrentSpeed, RuntimeMovement.TargetSpeed, RuntimeMovement.Acceleration * Time.deltaTime);
            }
            else
            {
                if (RuntimeMovement.CurrentSpeed < 0.001f)
                {
                    RuntimeMovement.CurrentSpeed = 0;
                }
                else
                {

                    RuntimeMovement.CurrentSpeed = Mathf.MoveTowards(RuntimeMovement.CurrentSpeed, 0, RuntimeMovement.Deceleration * Time.deltaTime);
                }
            }
        }
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

        public void AddMovementState(MovementStateBase state)
        {
            m_movementStates[state.GetType()] = state;

            MovementStateMachine.AddState(state);
        }
        public void RemoveMovementState<T>() where T : MovementStateBase
        {
            m_movementStates.Remove(typeof(T));

            MovementStateMachine.RemoveState<T>();
        }
        public T GetMovementState<T>() where T : MovementStateBase
        {
            if (m_movementStates.TryGetValue(typeof(T), out var state))
                return state as T;

            return null;
        }

        public bool HasMovementState<T>() where T : MovementStateBase
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
            if (RuntimeMovement.TargetDirection.magnitude > 0)
            {
                localDir = Motor.transform.InverseTransformDirection(RuntimeMovement.TargetDirection);
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
                if (forwardDot > 0.01f)
                    direction = Direction.Forward;
                else
                    direction = Direction.Backward;
            }
            else if (Mathf.Abs(forwardDot) < Mathf.Abs(rightDot))
            {
                if (rightDot > 0.01f)
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

        public Direction GetInputRelativeDirection()
        {
            Direction direction;
            if (RotationStrategy is CharacterRelativeRotation)
            {
                if (Input.MovementInput.magnitude > 0)
                {
                    direction = Direction.Forward;

                }
                else
                {
                    direction = Direction.NoMovement;
                }
            }
            else
            {
                Vector3 dir = transform.right * Input.MovementInput.x + transform.forward * Input.MovementInput.y;
                float forward = Vector3.Dot(transform.forward, dir);
                float right = Vector3.Dot(transform.right, dir);
                if (Mathf.Abs(forward) > Mathf.Abs(right))
                {
                    if (forward > 0)
                        direction = Direction.Forward;
                    else
                        direction = Direction.Backward;
                }
                else
                {
                    if (right > 0)
                        direction = Direction.Right;
                    else
                        direction = Direction.Left;
                }
            }
            return direction;
        }
        #region IDamageable
        public void TakeDamage(DamageData damage)
        {
            HealthController.TakeDamage(damage);
        }
        #endregion
    }
}
