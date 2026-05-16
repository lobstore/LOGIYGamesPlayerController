using LOGIYGames.CharacterCore;
using LOGIYGames.Scripts.AI;
using UnityEngine;
using UnityEngine.AI;

namespace LOGIYGames.AI
{
    public class AIBrain : MonoModuleBase
    {
        public AIInputReader InputReader { get; private set; }

        [Header("References")]

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private AIStatesPresetBase statesPreset;
        public Vector2 MovementInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool EvadePressed { get; private set; }
        public bool CrouchPressed { get; private set; }
        public bool SprintPressed { get; private set; }
        public bool FocusPressed { get; private set; }


        [Header("Detection Settings")]
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float lostTargetTimeout = 5f;

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float minIdleDuration = 2f;
        [SerializeField] private float maxIdleDuration = 5f;
        [SerializeField] private float patrolArrivalThreshold = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool debugDraw = true;
        private string currentStateName;

        // State Machine
        private StateMachine _stateMachine;
        public Transform[] PatrolPoints => patrolPoints;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public StateMachine StateMachine => _stateMachine;

        public float LostTargetTimeout { get => lostTargetTimeout; set => lostTargetTimeout = value; }
        public float MinIdleDuration { get => minIdleDuration; set => minIdleDuration = value; }
        public float MaxIdleDuration { get => maxIdleDuration; set => maxIdleDuration = value; }
        public float PatrolArrivalThreshold { get => patrolArrivalThreshold; set => patrolArrivalThreshold = value; }

        public Transform Target;

        private void Awake()
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            // Configure NavMeshAgent for pathfinding only (no position/rotation update)
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.acceleration = 0;
            navMeshAgent.angularSpeed = 0;
            navMeshAgent.speed = 0;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoTraverseOffMeshLink = false;
            InputReader = new(this);
            var character = GetComponent<Character>();
            //character.OnControlReleased.AddListener(() =>
            //{
            //    character.UpdateInput(InputReader);
            //    character.RotationStrategy = new InputRelativeRotation(character);
            //    character.MovementStrategy = new InputRelativeMovement(character);

            //});

        }

        private void Start()
        {
            InitializeStateMachine();

        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine();
            statesPreset.Init(this);
        }

        public void AddTransition(IState from, IState to, System.Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }

        public bool HasLineOfSight()
        {
            if (Target == null) return false;

            Vector3 direction = Target.position - transform.position;
            float distance = direction.magnitude;

            if (distance > detectionRange) return false;

            direction.Normalize();

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out RaycastHit hit, distance))
            {
                return hit.transform == Target || hit.transform.IsChildOf(Target);
            }

            return true;
        }

        public bool IsTargetDetected()
        {
            if (Target == null) return false;

            float distance = Vector3.Distance(transform.position, Target.position);
            if (distance > detectionRange) return false;

            return HasLineOfSight();
        }


        public bool HasLostTarget()
        {
            if (Target == null) return true;
            return Vector3.Distance(transform.position, Target.position) > detectionRange * 1.5f;
        }

        public float GetDistanceToTarget()
        {
            return Target == null ? float.MaxValue : Vector3.Distance(transform.position, Target.position);
        }

        public Vector3 GetDirectionToTarget()
        {
            if (Target == null) return transform.forward;

            Vector3 direction = Target.position - transform.position;
            direction.y = 0;
            return direction.normalized;
        }
        public override void OnUpdate(float deltaTime)
        {

            base.OnUpdate(deltaTime);
            if (_stateMachine == null) return;
            currentStateName = _stateMachine.CurrentNode?.State?.GetType().Name ?? "None";
            FocusPressed = true ? Target != null : false;
            _stateMachine.Update();
            if (navMeshAgent != null && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                Vector3 desiredDirection = navMeshAgent.path.corners[1] - transform.position;
                desiredDirection.y = 0;
                MovementInput = new Vector2(desiredDirection.x, desiredDirection.z).normalized;

            }
            else
            {
                MovementInput = Vector2.zero;
            }
        }
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            if (_stateMachine == null) return;
            _stateMachine.FixedUpdate();


        }
        public void SetDestination(Vector3 destination)
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.destination != destination)
            {
                navMeshAgent.SetDestination(destination);
            }
        }

        public void ClearDestination()
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
            }
        }
        private void OnDrawGizmosSelected()
        {
            if (!debugDraw) return;

            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                Gizmos.color = Color.cyan;
                Vector3[] corners = navMeshAgent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }

                Gizmos.color = Color.yellow;
                for (int i = 0; i < corners.Length; i++)
                {
                    Gizmos.DrawWireSphere(corners[i], 0.2f);
                }
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] == null) continue;

                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.5f);

                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }

                if (patrolPoints.Length > 1 && patrolPoints[0] != null)
                {
                    Gizmos.DrawLine(
                        patrolPoints[patrolPoints.Length - 1].position,
                        patrolPoints[0].position
                    );
                }
            }
        }
    }
}
