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

        // Pathfinding
        private float _pathRecalculateTimer = 0f;
        private const float PATH_RECALCULATE_INTERVAL = 0.3f;

        // Stuck detection
        private Vector3 _lastPosition = Vector3.zero;
        private float _stuckTimer = 0f;
        private const float STUCK_THRESHOLD = 0.1f;
        private const float STUCK_TIMEOUT = 2f;

        // Airborne state (jump/fall)
        private bool _wasGrounded = true;

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
            navMeshAgent.speed = 0; // Speed is controlled via Character.SpeedMultiplier, not NavMeshAgent
            navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoTraverseOffMeshLink = false;
            navMeshAgent.autoBraking = false;

            if (aiInput == null)
            {
                Debug.LogError("AIBrain requires AIInputReader component");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            _lastPosition = transform.position;
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
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                _stateMachine.SetState(_idleState);
            }
            else
            {
                _stateMachine.SetState(_idleState);
            }
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
                target != null && IsTargetDetected());

            // ----- Patrol State Transitions -----
            AddTransition(_patrolState, _idleState, () =>
                _patrolState.HasReachedPatrolPoint());
            AddTransition(_patrolState, _chaseState, () =>
                target != null && IsTargetDetected());

            // ----- Chase State Transitions -----
            AddTransition(_chaseState, _attackState, () =>
                target != null && IsTargetInAttackRange());
            AddTransition(_chaseState, _patrolState, () =>
                HasLostTarget() && patrolPoints != null && patrolPoints.Length > 0);
            AddTransition(_chaseState, _idleState, () =>
                HasLostTarget() && (patrolPoints == null || patrolPoints.Length == 0));

            // ----- Attack State Transitions -----
            AddTransition(_attackState, _chaseState, () =>
                target != null && !IsTargetInAttackRange());
            AddTransition(_attackState, _patrolState, () =>
                HasLostTarget() && patrolPoints != null && patrolPoints.Length > 0);
            AddTransition(_attackState, _idleState, () =>
                HasLostTarget() && (patrolPoints == null || patrolPoints.Length == 0));
        }

        /// <summary>
        /// Helper method to add transition with inline predicate
        /// </summary>
        private void AddTransition(IState from, IState to, System.Func<bool> condition)
        {
            _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
        }

        /// <summary>
        /// Checks if target is detected (in range and line of sight)
        /// </summary>
        private bool IsTargetDetected()
        {
            if (target == null) return false;

            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance <= detectionRange)
            {
                return HasLineOfSight();
            }

            return false;
        }

        /// <summary>
        /// Checks if target is in attack range
        /// </summary>
        private bool IsTargetInAttackRange()
        {
            if (target == null) return false;

            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= attackRange;
        }

        /// <summary>
        /// Checks if AI has line of sight to target
        /// </summary>
        private bool HasLineOfSight()
        {
            if (target == null) return false;

            Vector3 direction = target.position - transform.position;
            float distance = direction.magnitude;

            if (distance > detectionRange) return false;

            direction.Normalize();

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out RaycastHit hit, distance))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return true;
        }

        /// <summary>
        /// Checks if target has been lost (timeout exceeded)
        /// </summary>
        private bool HasLostTarget()
        {
            if (target == null) return true;
            
            // Check if we can still detect target
            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance > detectionRange * 1.5f)
            {
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (_stateMachine == null) return;

            // NavMeshAgent position is now updated automatically
            // We use it only for path calculation, movement is handled by CharacterController

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

            // Reset position change from NavMeshAgent (we handle movement via CharacterController)
            // But keep the agent at our position for path calculation
            if (navMeshAgent != null && navMeshAgent.updatePosition)
            {
                // NavMeshAgent moves the transform, we need to use its velocity for input
                // but not let it actually move us
            }
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
            if (target != null && IsTargetInAttackRange())
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
            // Don't recalculate path while airborne
            if (!isGrounded)
            {
                _wasGrounded = false;
                return;
            }

            // Recalculate path immediately when just landed
            if (!_wasGrounded)
            {
                _wasGrounded = true;
                _pathRecalculateTimer = PATH_RECALCULATE_INTERVAL;
            }

            // Recalculate path with throttling
            _pathRecalculateTimer += Time.deltaTime;
            if (_pathRecalculateTimer >= PATH_RECALCULATE_INTERVAL)
            {
                _pathRecalculateTimer = 0f;

                if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.SetDestination(destination);
                }
            }
        }

        /// <summary>
        /// Gets direction from current NavMesh path
        /// </summary>
        public Vector3 GetPathDirection()
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                if (navMeshAgent.path.corners != null && navMeshAgent.path.corners.Length > 1)
                {
                    Vector3 nextWaypoint = navMeshAgent.path.corners[1];
                    Vector3 direction = nextWaypoint - transform.position;
                    direction.y = 0;
                    return direction.normalized;
                }
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Checks if AI is stuck and tries to recover
        /// </summary>
        public bool IsStuck()
        {
            float moveDistance = Vector3.Distance(transform.position, _lastPosition);
            
            if (moveDistance < STUCK_THRESHOLD)
            {
                _stuckTimer += Time.deltaTime;
                return _stuckTimer >= STUCK_TIMEOUT;
            }
            else
            {
                _stuckTimer = 0f;
                _lastPosition = transform.position;
                return false;
            }
        }

        /// <summary>
        /// Recalculates path immediately (used when stuck)
        /// </summary>
        public void RecalculatePath()
        {
            _pathRecalculateTimer = PATH_RECALCULATE_INTERVAL;
        }

        /// <summary>
        /// Draws gizmos for AI visualization in editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!debugDraw)
            {
                return;
            }

            // Draw NavMesh path
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                if (navMeshAgent.path.corners != null && navMeshAgent.path.corners.Length > 1)
                {
                    Gizmos.color = Color.cyan;
                    Vector3[] corners = navMeshAgent.path.corners;
                    for (int i = 0; i < corners.Length - 1; i++)
                    {
                        Gizmos.DrawLine(corners[i], corners[i + 1]);
                    }

                    // Draw waypoints
                    Gizmos.color = Color.yellow;
                    for (int i = 0; i < corners.Length; i++)
                    {
                        Gizmos.DrawWireSphere(corners[i], 0.2f);
                    }
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
