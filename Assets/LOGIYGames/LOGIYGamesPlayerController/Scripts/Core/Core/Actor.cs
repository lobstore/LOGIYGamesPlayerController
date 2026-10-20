using Alchemy.Inspector;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Enums;
using LOGIYGames.Shared.Extensions;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames.CharacterCore
{
    public partial class Actor : MonoModuleBase, IControllable, IDamageable
    {
        [SerializeReference][ReadOnly] ICharacterInputReader InputReader;
        [field: SerializeField][field: ReadOnly] public CharacterInput Input { get; private set; }
        [field: SerializeReference][field: ReadOnly] public IMovementStrategy MovementStrategy { get; set; }
        [field: SerializeReference][field: ReadOnly] public IRotationStrategy RotationStrategy { get; set; }
        [field: SerializeReference][field: ReadOnly] public IEventDispatcher EventBus { get; private set; } = new EventDispatcher();
        [field: SerializeField] public CameraTarget CameraTarget { get; private set; }
        [ReadOnly] public MovementData RuntimeMovement;

        #region Modules
        public MovementWrapperBase Motor { get; private set; }
        public SensorsModule Sensors { get; private set; }
        public EffectsController Effects { get; private set; }
        public HealthController HealthController { get; private set; }
        public StaminaController StaminaController { get; private set; }
        public StatsController Stats { get; private set; }
        public TargetingController TargetingController;
        #endregion

        [Header("State Machine Configuration")]
        public StateMachine MovementStateMachine { get; private set; }
        public MovementBuilder movementPreset;
        private Dictionary<Type, MovementStateBase> m_movementStates = new();
        public bool IsGrounded { get => Sensors.IsGrounded; }
        private void Awake()
        {
            Motor = GetComponent<MovementWrapperBase>();
            Sensors = GetComponent<SensorsModule>();
            HealthController = this.GetOrAddComponent<HealthController>();
            StaminaController = this.GetOrAddComponent<StaminaController>();
            Effects = this.GetOrAddComponent<EffectsController>();
            Stats = this.GetOrAddComponent<StatsController>();
            InitializeStateMachine();
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
            if (InputReader != null)
            {
                Input = InputReader.GetInput();
            }
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

            if (turnSpeed > 0f && RotationStrategy is not MousePlanarRotation)
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
        public void SetInput(ICharacterInputReader inputReader)
        {
            InputReader = inputReader;
        }
        public void ResetInput()
        {
            Input = new CharacterInput();
        }

        #endregion
        public Direction GetRelativeMovementDirection()
        {
            Vector3 localDir;
            localDir = Motor.transform.InverseTransformDirection(Motor.Velocity);
            Direction direction;
            if (localDir.magnitude == 0)
            {
                direction = Direction.NoMovement;
            }
            else
            {


                float forwardDot = Vector3.Dot(localDir, Vector3.forward);
                float rightDot = Vector3.Dot(localDir, Vector3.right);
                float upDot = Vector3.Dot(localDir, Vector3.up);
                if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
                {
                    if (forwardDot > 0.01f)
                        direction = Direction.Forward;
                    else
                        direction = Direction.Backward;
                }
                else
                {
                    if (rightDot > 0.01f)
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
            OnDamageTaken.Invoke(damage.From);
            HealthController.TakeDamage(damage);
        }
        public UnityEvent<IDamageable> OnDamageTaken = new();
        #endregion
    }
}
