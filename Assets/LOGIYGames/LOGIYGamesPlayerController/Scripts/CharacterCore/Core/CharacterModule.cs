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
        public InputCommandBuffer ComboBuffer { get; private set; }
        public ComboController ComboController { get; private set; }
        public WeaponController WeaponController { get; private set; }

        public List<AbilityData> Abilities = new();

        public AbilityController AbilityController { get; private set; }
        #endregion
        #region State Machine Configuration
        [Header("State Machine Configuration")]
        public MovementStatesPresetBase movementPreset;

        private readonly Dictionary<Type, CharacterMovementState> _movementStates = new();

        private StateMachine _movementStateMachine;
        public StateMachine MovementStateMachine => _movementStateMachine;

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

        public bool CanMove { get; set; } = true;
        #endregion
        #region Inpector Debug Variables
        private string _currentMovementStateName;
        private string _lastMovementTransition;
        private string _currentActionStateName;
        private string _lastActionTransition;
        [SerializeField] Color movementTargetDirectionArrowColor;
        #endregion

        #region Velocity Variables


        [Header("Movement Configuration")]


        public CharacterVelocityData VelocityData { get; private set; }


        public float BaseSpeed;
        /// <summary>
        /// Gets or sets the deceleration value.
        /// </summary>
        /// <remarks>
        /// Deceleration does not work if the <see cref="useInertia"/> variable is set to <c>false</c>.
        /// </remarks>
        public float Deceleration { get; set; }

        /// <summary>
        /// Gets or sets the acceleration value.
        /// </summary>
        /// <remarks>
        /// Acceleration does not work if the <see cref="useInertia"/> variable is set to <c>false</c>.
        /// </remarks>
        public float Acceleration { get; set; }

        /// <summary>
        /// Gets or sets the internal speed multiplier.
        /// </summary>
        /// <remarks>
        /// This multiplier can be used to adjust the movement speed in base conditions, such as when moving walk or sprint.
        /// </remarks>
        public float SpeedMultiplier { get; set; }

        public float TurnSmoothTime { get; set; } = 5f;

        public float CurrentSpeed => SpeedMultiplier * BaseSpeed;

        private float deltaYaw;
        private Quaternion targetRotation;
        private Vector3 targetDirection;
        private Vector3 scaledTargetDirection;
        public Quaternion TargetRotation { get => targetRotation; set => targetRotation = value; }
        public Vector3 TargetDirection { get => targetDirection; set => targetDirection = value; }
        public Vector3 ScaledTargetDirection { get => scaledTargetDirection; set => scaledTargetDirection = value; }

        #endregion

        #region Height Properties

        [field: SerializeField] public float Height { get; set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;

        #endregion

        #region Camera References
        public CameraTargetModule CameraTarget { get; set; }

        public Transform CameraLookAt => CameraTarget.CameraLookAt;
        public Transform CameraFollow => CameraTarget.CameraFollow;

        #endregion
        public float DeltaYaw { get => deltaYaw; set => deltaYaw = value; }


        public UnityEvent OnControlReleased { get; } = new();


        private void Awake()
        {
            EventBus = new EventDispatcher();
            CameraTarget = GetComponent<CameraTargetModule>();
            ComboBuffer = new InputCommandBuffer();
            WeaponController = new WeaponController(this);
            ComboController = new ComboController(this, ComboBuffer);
            AbilityController = new AbilityController(this);
            GetComponent<ComboBufferDebugView>().Buffer = ComboBuffer;

            Targeting = new();
            VelocityData = new();

            EventsSubscription();
        }
        private void Start()
        {

            m_motor.Height = Height;
            m_motor.Center = new Vector3(0, Height / 2.0f, 0);
            InitializeStateMachine();

        }
        #region DamagableSystem



        #endregion
        private void EventsSubscription()
        {
            EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        Jump(TargetDirection * evt.planarForce, transform.up * evt.verticalForce);
                        JumpCount++;
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
            EventBus.Subscribe<MantlingEvent>((evt) =>
            {

            });

        }
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            _movementStateMachine.FixedUpdate();
        }
        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            _movementStateMachine.LateUpdate();
            SmoothHeightChanging();
        }
        public override void OnUpdate(float deltaTime)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                AbilityController.SetAbility( Abilities[0]);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                AbilityController.SetAbility(Abilities[1]);
            }
            base.OnUpdate(deltaTime);
            UpdateVelocity();
            TargetRotation = RotationStrategy.GetRotation();
            TargetDirection = MovementStrategy.GetMovementDirection();
            CalculateDeltaYaw();
            _movementStateMachine.Update();
            StatesDebug();
            var velo = TargetDirection * BaseSpeed;
            if (velo.magnitude > 0)
            {
                DebugDraw.DrawArrow(transform.position, velo, movementTargetDirectionArrowColor);

            }
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

        /// <summary>
        /// Rotates character to face the desired direction.
        /// </summary>
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, m_motor.transform.up);
            Rotate(targetRotation, turnSmoothTime);
        }

        /// <summary>
        /// Rotates character to face a position.
        /// </summary>
        public void RotateToPosition(Vector3 position, float turnSmoothTime = 0)
        {
            Vector3 desiredDirection = position - m_motor.transform.position;
            RotateToDirection(desiredDirection.normalized, turnSmoothTime);
        }

        /// <summary>
        /// Rotates character to a target rotation.
        /// Delegates to controller wrapper for proper KinematicCharacterController integration.
        /// </summary>
        public void Rotate(Quaternion targetRotation, float turnSpeed = 0)
        {
            if (turnSpeed > 0f)
            {
                // Smooth rotation using Slerp
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
            deltaYaw = Mathf.DeltaAngle(transform.eulerAngles.y, TargetRotation.eulerAngles.y);
            if (Mathf.Abs(deltaYaw) < 0.01f)
            {
                deltaYaw = 0;
            }
        }

        #endregion
        #region Movement Methods

        public void Move(Vector3 moveDirection)
        {
            m_motor.Move(VelocityData.Locomotion);
        }

        private void UpdateVelocity()
        {
            if (TargetDirection.magnitude > 0)
            {

                ScaledTargetDirection = Vector3.Lerp(ScaledTargetDirection, TargetDirection.normalized, Acceleration * Time.deltaTime);
                VelocityData.Locomotion = Vector3.Lerp(VelocityData.Locomotion, targetDirection.normalized * CurrentSpeed, Acceleration * Time.deltaTime);
            }
            else
            {
                ScaledTargetDirection = Vector3.Lerp(ScaledTargetDirection, Vector3.zero, Deceleration * Time.deltaTime);
                VelocityData.Locomotion = Vector3.Lerp(VelocityData.Locomotion, Vector3.zero, Deceleration * Time.deltaTime);
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
            return Vector3.ProjectOnPlane(VelocityData.Locomotion, Sensors.BelowHit.normal) + Vector3.ProjectOnPlane(-m_motor.transform.up * SpeedMultiplier, Sensors.BelowHit.normal);
        }
        public void Slide()
        {
            m_motor.Move(ProjectVelocity());
        }

        public void ResetVelocity()
        {
            VelocityData.Reset();
            TargetDirection = Vector3.zero;
            ScaledTargetDirection = Vector3.zero;
            m_motor.ResetVelocity();
        }
        public void ResetSpeed()
        {
            SpeedMultiplier = 0;
        }
        #endregion
        private void InitializeStateMachine()
        {
            _movementStateMachine = new StateMachine();
            if (movementPreset != null)
            {
                movementPreset.Init(this);

            }
            else
            {
                Debug.LogError("No MovementPreset provided");
            }
        }
        #region Movement State Machine

        public void AddMovementState(CharacterMovementState state)
        {
            _movementStates[state.GetType()] = state;

            MovementStateMachine.AddState(state);
        }
        public void RemoveMovementState<T>() where T : CharacterMovementState
        {
            _movementStates.Remove(typeof(T));

            MovementStateMachine.RemoveState<T>();
        }
        public T GetMovementState<T>() where T : CharacterMovementState
        {
            if (_movementStates.TryGetValue(typeof(T), out var state))
                return state as T;

            return null;
        }

        public bool HasMovementState<T>() where T : CharacterMovementState
        {
            return _movementStates.ContainsKey(typeof(T));
        }
        #endregion
        #region Action State Machine
        #endregion
        private void StatesDebug()
        {
            _currentMovementStateName = _movementStateMachine.CurrentNode.State.ToString();
            _lastMovementTransition = _movementStateMachine.LastTransition;
        }

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
            // Сравниваем проекции, чтобы определить направление
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

    [Serializable]
    public class CharacterTargetingModule
    {
        public Transform CurrentTarget { get; private set; }

        public bool HasTarget =>
            CurrentTarget != null;

        public void SetTarget(Transform target)
        {
            CurrentTarget = target;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
        }
    }
}
