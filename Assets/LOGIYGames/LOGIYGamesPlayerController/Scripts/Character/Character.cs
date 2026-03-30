using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;



namespace LOGIYGames.CharacterCore
{
    public class JumpPerformedEvent
    {
        public enum Direction
        {
            Left,
            Right,
            Forward,
            Backward
        }
        public Direction direction;
        public float verticalForce;
        public float planarForce;
    }
    public class DashPerformedEvent : JumpPerformedEvent
    {


    }
    public class RollPerformedEvent : JumpPerformedEvent
    {
    }
    public class TurnPerformedEvent
    {
        public float movementSpeed;
        public float angle;
    }
    public class LeashWeaponEvent
    {
        public bool unleashWeapon;
    }
    public class ItemThrowedEvent
    {

    }
    public class LandedEvent
    {

    }
    public class Character : MonoModuleBase, IControllable
    {
        public IMovementInputReader Input { get; set; }
        [Header("References")]

        [field: SerializeField] private ControllerWrapperBase Motor;
        [field: SerializeField] public SensorsModule Sensors { get; private set; }
        // TODO Make Builder
        public IMovementStrategy CurrentMovementStrategy { get; set; }
        public IRotationStrategy CurrentRotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }

        public IEventDispatcher EventBus { get; private set; }

        public int JumpCount;

        public Transform Target;

        [Header("State Machine Configuration")]
        public MovementStatesPresetBase movementPreset;
        private StateMachine _movementStateMachine;
        private StateMachine _actionStateMachine;

        public StateMachine MovementStateMachine => _movementStateMachine;
        public StateMachine ActionStateMachine => _actionStateMachine;

        public bool IsFalling { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsGrounded { get => Motor.IsGrounded; }
        public bool IsSliding { get; set; }

        public void SetInputReader(IMovementInputReader inputReader)
        {
            Input = inputReader;
        }

        #region Debug

        private string _currentMovementStateName;
        private string _lastMovementTransition;
        private string _currentActionStateName;
        private string _lastActionTransition;
        #endregion
        #region VelocityVariables
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
        public Transform CameraLookAt;
        public Transform CameraFollow;
        public float DeltaYaw { get => deltaYaw; set => deltaYaw = value; }

        #endregion
        private void Awake()
        {
            CurrentMovementStrategy = new CameraRelativeMovement(this);
            CurrentRotationStrategy = new CameraRelativeRotation(this);
            DefaultRotationStrategy = CurrentRotationStrategy;
            EventBus = new EventDispatcher();
            InitializeStateMachine();
        }
        private void Start()
        {
            // TODO Make ICBFollowable abstraction to change follow target
            CameraManager.Instance.SetTargetTo(CameraFollow, CameraLookAt);
            Motor.Height = Height;
            Motor.Center = new Vector3(0, Height / 2.0f, 0);

            EventBus.Subscribe<JumpPerformedEvent>(Jump);
            EventBus.Subscribe<RollPerformedEvent>(Roll);
            EventBus.Subscribe<DashPerformedEvent>(Dash);

        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ladder")) Target = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ladder")) Target = null;
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

            targetRotation = CurrentRotationStrategy.GetRotation();
            CalculateDeltaYaw();
            targetDirection = CurrentMovementStrategy.GetMovementDirection();
            _movementStateMachine.Update();
            _actionStateMachine.Update();
            Debug();


        }

        private void Debug()
        {
            _currentMovementStateName = _movementStateMachine.CurrentNode.State.ToString();
            _lastMovementTransition = _movementStateMachine.LastTransition;
            _currentActionStateName = _actionStateMachine.CurrentNode.State.ToString();
            _lastActionTransition = _actionStateMachine.LastTransition;
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

        #endregion

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

    }
}
