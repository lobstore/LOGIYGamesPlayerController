using LOGIYGames.Shared.Character.Events;
using System;
using UnityEngine;
using UnityEngine.Events;



namespace LOGIYGames.CharacterCore
{

    public class Character : MonoModuleBase, IControllable
    {
        public ICharacterInputReader Input { get; set; }
        [Header("References")]

        [field: SerializeField] private ControllerWrapperBase Motor;
        [field: SerializeField] public SensorsModule Sensors { get; private set; }
        public IMovementStrategy MovementStrategy { get; set; }
        public IRotationStrategy RotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }
        public IMovementStrategy DefaultMovementStrategy { get; set; }
        public IEventDispatcher EventBus { get; private set; }

        public int JumpCount;

        [Header("State Machine Configuration")]
        #region State Machine Configuration
        public MovementStatesPresetBase movementPreset;
        private StateMachine _movementStateMachine;
        private StateMachine _actionStateMachine;
        public StateMachine MovementStateMachine => _movementStateMachine;
        public StateMachine ActionStateMachine => _actionStateMachine;
        #endregion

        public bool IsFalling { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsGrounded { get => Motor.IsGrounded; }
        public bool IsSliding { get; set; }
        public bool IsOnLadder { get; set; }

        #region Inpector Debug Variables
        private string _currentMovementStateName;
        private string _lastMovementTransition;
        private string _currentActionStateName;
        private string _lastActionTransition;
        #endregion

        #region Velocity Variables
        [Header("Movement Configuration")]
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

        public float CurrentSpeed => SpeedMultiplier * BaseSpeed * Input.MovementInput.magnitude;

        public Vector3 Velocity { get => velocity; set => velocity = value; }

        private Vector3 velocity;
        private float deltaYaw;
        public Quaternion targetRotation { get; set; }
        public Vector3 targetDirection { get; set; }

        #endregion

        #region Height Properties

        [field: SerializeField] public float Height { get; set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;

        #endregion

        #region Camera References
        [Header("Camera Configuration")]
        [SerializeField] Transform cameraLookAt;
        [SerializeField] Transform cameraFollow;
        public Transform CameraLookAt => cameraLookAt;
        public Transform CameraFollow => cameraFollow;
        public float DeltaYaw { get => deltaYaw; set => deltaYaw = value; }
        #endregion

        public UnityEvent OnControlReleased { get; } = new();
        private void Awake()
        {
            EventBus = new EventDispatcher();
            InitializeStateMachine();
            DefaultMovementStrategy = new CameraRelativeMovement(this);
            DefaultRotationStrategy = new CameraRelativeRotation(this);
            EventsSubscription();
        }
        private void Start()
        {

            Motor.Height = Height;
            Motor.Center = new Vector3(0, Height / 2.0f, 0);

            if (Input == null)
            {
                Release();

            }
        }

        private void EventsSubscription()
        {
            EventBus.Subscribe<JumpPerformedEvent>(Jump);
            EventBus.Subscribe<RollPerformedEvent>(Roll);
            EventBus.Subscribe<DashPerformedEvent>(Dash);
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            _movementStateMachine.FixedUpdate();
            _actionStateMachine.FixedUpdate();
        }
        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            _movementStateMachine.LateUpdate();
            _actionStateMachine.LateUpdate();
            SmoothHeightChanging();
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            targetRotation = RotationStrategy.GetRotation();
            targetDirection = MovementStrategy.GetMovementDirection().normalized;
            CalculateDeltaYaw();
            _movementStateMachine.Update();
            _actionStateMachine.Update();
            StatesDebug();


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

        /// <summary>
        /// Rotates character to face the desired direction.
        /// </summary>
        public void RotateToDirection(Vector3 desiredDirection, float turnSmoothTime = 0)
        {
            desiredDirection.y = 0;
            if (desiredDirection.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            Rotate(targetRotation, turnSmoothTime);
        }

        /// <summary>
        /// Rotates character to face a position.
        /// </summary>
        public void RotateToPosition(Vector3 position, float turnSmoothTime = 0)
        {
            Vector3 desiredDirection = position - transform.position;
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
                Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSmoothTime * Time.fixedDeltaTime);
                Motor.Rotate(smoothedRotation);
            }
            else
            {
                Motor.Rotate(targetRotation);
            }
        }

        private void CalculateDeltaYaw()
        {
            deltaYaw = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);
            if (Mathf.Abs(deltaYaw) < 0.01f)
            {
                deltaYaw = 0;
            }
        }

        #endregion
        #region Movement Methods

        public void Move(Vector3 moveDirection)
        {
            if (Input.MovementInput.magnitude > 0)
            {
                Velocity = Vector3.Lerp(Velocity, moveDirection.normalized * CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {
                if (Velocity.magnitude > 0.001f)
                {

                    Velocity = Vector3.Lerp(Velocity, Vector3.zero, Deceleration * Time.deltaTime);
                }
                else
                {
                    Velocity = Vector3.zero;
                }
            }
            Motor.Move(Velocity);
        }
        public void SetPosition(Vector3 position)
        {
            Motor.SetPosition(position);
        }
        public void Slide()
        {
            Motor.Move(ProjectVelocity());
        }
        public void Dash(DashPerformedEvent evt)
        {
            // --- Добавляем импульс для рывка ---
            Velocity += targetDirection * evt.planarForce;


        }
        private Vector3 ProjectVelocity()
        {
            return Vector3.ProjectOnPlane(Velocity, Sensors.BelowHit.normal) + Vector3.ProjectOnPlane(-transform.up * SpeedMultiplier, Sensors.BelowHit.normal);
        }
        public void Jump(JumpPerformedEvent evt)
        {
            Velocity += targetDirection * SpeedMultiplier * evt.planarForce;
            Motor.Jump(evt.verticalForce);
            JumpCount++;
        }
        public void Roll(RollPerformedEvent evt)
        {
            Velocity = transform.forward * evt.planarForce + Velocity;
            Motor.Jump(evt.verticalForce);
        }

        public void ResetVelocity()
        {
            Acceleration = 0;
            Deceleration = 0;
            SpeedMultiplier = 0;
            velocity = Vector3.zero;
            targetDirection = Vector3.zero;
        }
        #endregion
        #region State Machine
        private void InitializeStateMachine()
        {
            _movementStateMachine = new StateMachine();
            _actionStateMachine = new StateMachine();
            if (movementPreset != null)
            {
                movementPreset.Init(this);

            }
            else
            {
                UnityEngine.Debug.LogError("No MovementPreset provided");
            }
        }
        public void AddStateMachineTransition(IState from, IState to, Func<bool> condition)
        {
            _movementStateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }
        public void AddSubStateMachineTransition(IState from, IState to, Func<bool> condition)
        {
            _actionStateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }
        public void AddAnyStateMachineTransition(IState to, Func<bool> condition)
        {
            _movementStateMachine.AddAnyTransition(to, new FuncPredicate(condition));
        }
        public void AddAnySubStateMachineTransition(IState to, Func<bool> condition)
        {
            _actionStateMachine.AddAnyTransition(to, new FuncPredicate(condition));
        }
        private void StatesDebug()
        {
            _currentMovementStateName = _movementStateMachine.CurrentNode.State.ToString();
            _lastMovementTransition = _movementStateMachine.LastTransition;
            _currentActionStateName = _actionStateMachine.CurrentNode.State.ToString();
            _lastActionTransition = _actionStateMachine.LastTransition;
        }
        #endregion
        #region IControllable
        public void TakeControl(ICharacterInputReader inputReader)
        {
            Input = inputReader;
        }

        public void Release()
        {
            Input = new NoneInput();
            RotationStrategy = new NoneRotation();
            MovementStrategy = new NoneMovement();
            OnControlReleased.Invoke();
        }
        #endregion
    }
}
