using LOGIYGames.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Brain component that manages AI behavior state machine
    /// Similar to MovementStateDriver but for AI behavior states
    ///
    /// State Transitions:
    /// - Idle <-> Patrol (based on configuration and time)
    /// - Idle/Patrol -> Chase (when target detected)
    /// - Chase -> Attack (when target in attack range)
    /// - Chase -> Idle/Patrol (when target lost)
    /// - Attack -> Chase (when target out of range)
    /// </summary>
    public class AIBrain : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AIInputReader aiInput;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [Header("Detection Settings")]
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float lostTargetTimeout = 5f;

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float minIdleDuration = 2f;
        [SerializeField] private float maxIdleDuration = 5f;
        [SerializeField] private float patrolArrivalThreshold = 0.5f;

        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Debug")]
        [SerializeField] private bool debugDraw = true;
        private string currentStateName;

        // Helpers
        private AIPathfinder _pathfinder;
        private AIDetector _detector;
        private AIStuckDetector _stuckDetector;

        // State Machine
        private StateMachine _stateMachine;

        // AI States
        private AIIdleState _idleState;
        private AIPatrolState _patrolState;
        private AIChaseState _chaseState;
        private AIAttackState _attackState;

        public AIInputReader AIInput => aiInput;
        public Transform Target => target;
        public Transform[] PatrolPoints => patrolPoints;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public StateMachine StateMachine => _stateMachine;

        private void Awake()
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            if (navMeshAgent == null)
            {
                Debug.LogError("AIBrain requires NavMeshAgent component");
                enabled = false;
                return;
            }

            // NavMeshAgent is used for pathfinding only, not for movement
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.acceleration = 9999;
            navMeshAgent.angularSpeed = 9999;
            navMeshAgent.speed = 0f;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoTraverseOffMeshLink = false;
            navMeshAgent.autoBraking = false;

            if (aiInput == null)
            {
                Debug.LogError("AIBrain requires AIInputReader component");
                enabled = false;
                return;
            }

            // Initialize helpers
            _pathfinder = new AIPathfinder(navMeshAgent, transform);
            _detector = new AIDetector(transform, detectionRange, attackRange);
            _stuckDetector = new AIStuckDetector();
        }

        private void Start()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine();

            // Initialize all states
            _idleState = new AIIdleState(this, minIdleDuration, maxIdleDuration);
            _patrolState = new AIPatrolState(this, patrolArrivalThreshold);
            _chaseState = new AIChaseState(this, lostTargetTimeout);
            _attackState = new AIAttackState(this);

            // Configure all transitions
            ConfigureTransitions();

            // Set initial state
            _stateMachine.SetState(patrolPoints != null && patrolPoints.Length > 0 ? _idleState : _idleState);
        }

        /// <summary>
        /// Configures all AI behavior state transitions
        /// </summary>
        private void ConfigureTransitions()
        {
            // ============================================
            // AI BEHAVIOR TRANSITION TABLE
            // ============================================
            // From State     | To State      | Condition
            // --------------------------------------------
            // Idle           | Patrol        | Idle timer finished & has patrol points
            // Idle           | Chase         | Target detected
            // --------------------------------------------
            // Patrol         | Idle          | Reached patrol point
            // Patrol         | Chase         | Target detected
            // --------------------------------------------
            // Chase          | Attack        | Target in attack range
            // Chase          | Idle          | Target lost (no patrol points)
            // Chase          | Patrol        | Target lost (has patrol points)
            // --------------------------------------------
            // Attack         | Chase         | Target out of attack range
            // Attack         | Idle          | Target lost (no patrol points)
            // Attack         | Patrol        | Target lost (has patrol points)
            // ============================================

            // ----- Idle State Transitions -----
            AddTransition(_idleState, _patrolState, () =>
                patrolPoints != null && patrolPoints.Length > 0 && _idleState.IsIdleComplete());
            AddTransition(_idleState, _chaseState, () =>
                target != null && _detector.IsTargetDetected(target));

            // ----- Patrol State Transitions -----
            AddTransition(_patrolState, _idleState, () =>
                _patrolState.HasReachedPatrolPoint());
            AddTransition(_patrolState, _chaseState, () =>
                target != null && _detector.IsTargetDetected(target));

            // ----- Chase State Transitions -----
            AddTransition(_chaseState, _attackState, () =>
                target != null && _detector.IsTargetInAttackRange(target));
            AddTransition(_chaseState, _patrolState, () =>
                _detector.HasLostTarget(target) && patrolPoints != null && patrolPoints.Length > 0);
            AddTransition(_chaseState, _idleState, () =>
                _detector.HasLostTarget(target) && (patrolPoints == null || patrolPoints.Length == 0));

            // ----- Attack State Transitions -----
            AddTransition(_attackState, _chaseState, () =>
                target != null && !_detector.IsTargetInAttackRange(target));
            AddTransition(_attackState, _patrolState, () =>
                _detector.HasLostTarget(target) && patrolPoints != null && patrolPoints.Length > 0);
            AddTransition(_attackState, _idleState, () =>
                _detector.HasLostTarget(target) && (patrolPoints == null || patrolPoints.Length == 0));
        }

        /// <summary>
        /// Helper method to add transition with inline predicate
        /// </summary>
        private void AddTransition(IState from, IState to, System.Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }

        /// <summary>
        /// Checks if AI has line of sight to target
        /// </summary>
        public bool HasLineOfSight()
        {
            return _detector.HasLineOfSight(target, detectionRange);
        }

        private void Update()
        {
            if (_stateMachine == null) return;

            currentStateName = _stateMachine.CurrentNode?.State?.GetType().Name ?? "None";
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            if (_stateMachine == null) return;
            _stateMachine.FixedUpdate();
        }

        private void LateUpdate()
        {
            if (_stateMachine == null) return;
            _stateMachine.LateUpdate();
        }

        /// <summary>
        /// Sets a new target for the AI to chase/attack
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Clears the current target
        /// </summary>
        public void ClearTarget()
        {
            target = null;
        }

        /// <summary>
        /// Sets patrol points for the AI
        /// </summary>
        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
        }

        /// <summary>
        /// Forces a state change to Idle
        /// </summary>
        public void GoToIdle()
        {
            _stateMachine.ChangeState(_idleState);
        }

        /// <summary>
        /// Forces a state change to Patrol
        /// </summary>
        public void GoToPatrol()
        {
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                _stateMachine.ChangeState(_patrolState);
            }
            else
            {
                Debug.LogWarning("Cannot go to Patrol state - no patrol points set");
                GoToIdle();
            }
        }

        /// <summary>
        /// Forces a state change to Chase
        /// </summary>
        public void GoToChase()
        {
            if (target != null)
            {
                _stateMachine.ChangeState(_chaseState);
            }
            else
            {
                Debug.LogWarning("Cannot go to Chase state - no target set");
            }
        }

        /// <summary>
        /// Forces a state change to Attack
        /// </summary>
        public void GoToAttack()
        {
            if (target != null && _detector.IsTargetInAttackRange(target))
            {
                _stateMachine.ChangeState(_attackState);
            }
            else
            {
                Debug.LogWarning("Cannot go to Attack state - target not in range");
            }
        }

        /// <summary>
        /// Gets the current behavior state name
        /// </summary>
        public string GetCurrentStateName()
        {
            return currentStateName;
        }

        /// <summary>
        /// Updates NavMesh path to destination
        /// </summary>
        public void UpdatePath(Vector3 destination, bool isGrounded = true)
        {
            _pathfinder.UpdatePath(destination, isGrounded);
        }

        /// <summary>
        /// Gets direction from current NavMesh path
        /// </summary>
        public Vector3 GetPathDirection()
        {
            return _pathfinder.GetDirection();
        }

        /// <summary>
        /// Checks if AI is stuck and tries to recover
        /// </summary>
        public bool IsStuck()
        {
            return _stuckDetector.IsStuck(transform.position);
        }

        /// <summary>
        /// Recalculates path immediately (used when stuck)
        /// </summary>
        public void RecalculatePath()
        {
            _pathfinder.Recalculate();
            _stuckDetector.Reset();
        }

        /// <summary>
        /// Gets distance to target
        /// </summary>
        public float GetDistanceToTarget()
        {
            return _detector.GetDistanceToTarget(target);
        }

        /// <summary>
        /// Gets direction to target
        /// </summary>
        public Vector3 GetDirectionToTarget()
        {
            return _detector.GetDirectionToTarget(target);
        }

        /// <summary>
        /// Draws gizmos for AI visualization in editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!debugDraw) return;

            // Draw NavMesh path
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

            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Draw patrol points
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
