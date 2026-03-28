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
        [Header("References")]
        public IMovementInputReader Input { get; set; }

        [field: SerializeField] private ControllerWrapperBase CController;
        [field: SerializeField] public SensorsModule Sensors { get; private set; }
        // TODO Make Builder
        public IMovementStrategy CurrentMovementStrategy { get; set; }
        public IRotationStrategy CurrentRotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }

        public IEventDispatcher EventBus { get; private set; }

        public int JumpCount;

        [Header("State Machine Configuration")]
        public MovementStatesPresetBase movementPreset;
        private StateMachine _movementStateMachine;
        private StateMachine _actionStateMachine;

        public StateMachine MovementStateMachine => _movementStateMachine;
        public StateMachine ActionStateMachine => _actionStateMachine;

        public bool IsFalling { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsGrounded { get => CController.IsGrounded; }
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

        [field: SerializeField] public float BaseSpeed { get; set; }
        public float CurrentSpeed => SpeedMultiplier * BaseSpeed * Input.MovementInput.magnitude;

        public Vector3 Velocity { get => velocity; set => velocity = value; }

        private Vector3 velocity;
        private float deltaYaw;


        #endregion
        public Quaternion targetRotation;
        public Vector3 targetDirection;
        #region Height Properties

        [field: SerializeField] public float Height { get; set; }
        public float HeightChangingSmoothTime { get; private set; } = 4f;

        #endregion

        #region Camera References

        [field: SerializeField] public Transform CinemachineCameraLookAtTransform { get; set; }
        [field: SerializeField] public Transform CinemachineCameraFollowTransform { get; set; }
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
            CameraManager.Instance.SetTargetTo(CinemachineCameraFollowTransform, CinemachineCameraLookAtTransform);
            CController.Height = Height;
            CController.Center = new Vector3(0, Height / 2.0f, 0);

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
            if (Height == CController.Height && CController.Center.y == Height) return;
            if (!Mathf.Approximately(CController.Height, Height) || !Mathf.Approximately(CController.Center.y, Height / 2.0f))
            {
                CController.Height = Mathf.Lerp(CController.Height, Height, HeightChangingSmoothTime * Time.deltaTime);
                CController.Center = Vector3.Lerp(CController.Center, new Vector3(0, Height / 2.0f, 0), HeightChangingSmoothTime * Time.deltaTime);
            }
            else
            {
                CController.Height = Height;
                CController.Center = new Vector3(0, Height / 2.0f, 0);
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
            Rotate(targetRotation);
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
        public void Rotate(Quaternion targetRotation)
        {
            if (TurnSmoothTime > 0f)
            {
                // Smooth rotation using Slerp
                Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSmoothTime * Time.fixedDeltaTime);
                CController.Rotate(smoothedRotation);
            }
            else
            {
                CController.Rotate(targetRotation);
            }
        }
        public void Rotate()
        {
            Rotate(targetRotation);
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
                if (Velocity.magnitude > 0.01f)
                {

                    Velocity = Vector3.Lerp(Velocity, Vector3.zero, Deceleration * Time.deltaTime);
                }
                else
                {
                    Velocity = Vector3.zero;
                }
            }
            CController.Move(Velocity);
        }
        public void Move()
        {
            Move(targetDirection);
        }
        public void Slide()
        {
            CController.Move(ProjectVelocity());
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
            CController.Jump(evt.verticalForce);
            JumpCount++;
        }
        public void Roll(RollPerformedEvent evt)
        {
            Velocity = transform.forward * evt.planarForce + Velocity;
            CController.Jump(evt.verticalForce);
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
