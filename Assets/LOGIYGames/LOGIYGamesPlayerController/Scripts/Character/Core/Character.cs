using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;



namespace LOGIYGames.CharacterCore
{

    public class Character : MonoModuleBase, IControllable
    {
        public ICharacterInputReader InputProvider { get; set; }
        public CharacterInput Input { get; private set; }
        public IMovementStrategy MovementStrategy { get; set; }
        public IRotationStrategy RotationStrategy { get; set; }
        public IRotationStrategy DefaultRotationStrategy { get; set; }
        public IMovementStrategy DefaultMovementStrategy { get; set; }
        public IEventDispatcher EventBus { get; private set; }

        [Header("References")]


        public CharacterTargetingModule Targeting { get; private set; }


        [field: SerializeField] private ControllerWrapperBase m_motor;
        [field: SerializeField] public SensorsModule Sensors { get; private set; }

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
        public bool IsFlying { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsGrounded { get => Sensors.IsGrounded; }
        public bool IsSliding { get; set; }
        public bool IsOnLadder { get; set; }
        public bool IsWallClimbing { get; set; }
        public bool IsSwimming { get; set; }

        #region Inpector Debug Variables
        private string _currentMovementStateName;
        private string _lastMovementTransition;
        private string _currentActionStateName;
        private string _lastActionTransition;
        [SerializeField] Color movementTargetDirectionArrowColor;
        #endregion

        #region Velocity Variables


        [Header("Movement Configuration")]
        
        
        public CharacterVelocityData VelocityData {  get; private set; }


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
            Targeting = new();
            VelocityData = new();
            if (InputProvider == null)
            {
                ReleaseControl();

            }
            Targeting.SetTarget( target);
            EventsSubscription();
        }
        private void Start()
        {

            m_motor.Height = Height;
            m_motor.Center = new Vector3(0, Height / 2.0f, 0);
            InitializeStateMachine();

        }
        private void EventsSubscription()
        {
            EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        GroundJump(evt);
                        break;
                    case JumpType.HangJump:
                        WallJump(evt);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    default:
                        break;
                }

            });
            EventBus.Subscribe<RollPerformedEvent>(Roll);
            EventBus.Subscribe<DashPerformedEvent>(Dash);
            EventBus.Subscribe<SlipPerformedEvent>(SlipJump);
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
            Debug.Log(RotationStrategy.GetType());
            ReadInput();
            targetRotation = RotationStrategy.GetRotation();
            CalculateDeltaYaw();
            targetDirection = MovementStrategy.GetMovementDirection();
            _movementStateMachine.Update();
            _actionStateMachine.Update();
            StatesDebug();
            DebugDraw.DrawArrow(transform.position, targetDirection * BaseSpeed, movementTargetDirectionArrowColor);

        }
        public void Aim()
        {

        }
        [SerializeField] Transform target;

        private Transform FindNearestTarget()
        {
            Collider[] hits =
                Physics.OverlapSphere(
                    transform.position,
                    15f);

            Transform bestTarget = null;

            float bestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                float dist =
                    Vector3.Distance(
                        transform.position,
                        hit.transform.position);

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestTarget = hit.transform;
                }
            }

            return bestTarget;
        }
        private void ReadInput()
        {
            if (InputProvider==null)
            {
                return;
            }
            Input = InputProvider.GetInput();
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
                VelocityData.Locomotion = Vector3.Lerp(VelocityData.Locomotion, moveDirection.normalized * CurrentSpeed, Acceleration * Time.deltaTime);

            }
            else
            {
                if (VelocityData.Locomotion.magnitude > 0.0001f)
                {

                    VelocityData.Locomotion = Vector3.Lerp(VelocityData.Locomotion, Vector3.zero, Deceleration * Time.deltaTime);
                }
                else
                {
                    VelocityData.Locomotion = Vector3.zero;
                }
            }
            m_motor.Move(VelocityData.Locomotion);
        }
        public void ForceMove(Vector3 moveDirection)
        {
            m_motor.ForceMove(moveDirection);
        }
        public void SetPosition(Vector3 position)
        {
            m_motor.SetPosition(position);
        }
        public void Slide()
        {
            m_motor.Move(ProjectVelocity());
        }
        private void SlipJump(SlipPerformedEvent evt)
        {
            VelocityData.Locomotion = m_motor.transform.forward * evt.planarForce;
        }
        public void Dash(DashPerformedEvent evt)
        {
            VelocityData.Locomotion = targetDirection * evt.planarForce;
            m_motor.Jump(new Vector3(0, evt.verticalForce, 0));
        }
        private Vector3 ProjectVelocity()
        {
            return Vector3.ProjectOnPlane(VelocityData.Locomotion, Sensors.BelowHit.normal) + Vector3.ProjectOnPlane(-m_motor.transform.up * SpeedMultiplier, Sensors.BelowHit.normal);
        }
        public void GroundJump(JumpPerformedEvent evt)
        {
            VelocityData.Locomotion = targetDirection * evt.planarForce;
            m_motor.Jump(new Vector3(0, evt.verticalForce, 0));
            JumpCount++;
        }
        public void WallJump(JumpPerformedEvent evt)
        {
            VelocityData.Locomotion = Sensors.LegsFrontHit.normal * evt.planarForce;
            m_motor.Jump(new Vector3(0, evt.verticalForce, 0));
            JumpCount++;
        }
        public void Roll(RollPerformedEvent evt)
        {
            VelocityData.Locomotion = m_motor.transform.forward * evt.planarForce;
            m_motor.Jump(new Vector3(0, evt.verticalForce, 0));
        }

        public void ResetVelocity()
        {
            VelocityData.Reset();
            targetDirection = Vector3.zero;
            m_motor.ResetVelocity();
        }
        public void ResetSpeed()
        {
            SpeedMultiplier = 0;
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
        public void AddMovementStateMachineTransition(CharacterMovementState from, CharacterMovementState to, Func<bool> condition)
        {
            _movementStateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }
        public void AddAnyMovementStateMachineTransition(CharacterMovementState to, Func<bool> condition)
        {
            _movementStateMachine.AddAnyTransition(to, new FuncPredicate(condition));
        }
        public void AddActionStateMachineTransition(ActionBaseState from, ActionBaseState to, Func<bool> condition)
        {
            _actionStateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }
        public void AddAnyActionStateMachineTransition(ActionBaseState to, Func<bool> condition)
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
            InputProvider = inputReader;
            ResetStrategies();
        }

        public void ResetStrategies()
        {
            RotationStrategy = DefaultRotationStrategy;
            MovementStrategy = DefaultMovementStrategy;
        }

        public void ReleaseControl()
        {
            InputProvider = new NoneInput();
            RotationStrategy = new NoneRotation(this);
            MovementStrategy = new NoneMovement();
            OnControlReleased.Invoke();
        }
        #endregion
        public Direction GetRelativeMovementDirection()
        {
            Vector3 localDir;
            if (targetDirection.magnitude > 0)
            {
                localDir = m_motor.transform.InverseTransformDirection(targetDirection);
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
