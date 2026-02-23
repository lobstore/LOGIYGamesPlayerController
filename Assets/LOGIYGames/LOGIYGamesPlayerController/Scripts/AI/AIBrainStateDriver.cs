using LOGIYGames.CharacterCore;
using LOGIYGames.Scripts.AI;
using UnityEngine;
using UnityEngine.AI;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Brain component that manages AI behavior state machine
    /// Uses NavMeshAgent directly for movement
    /// </summary>
    public class AIBrainStateDriver : MonoModuleBase
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent navMeshAgent;
        public AIInputReader Output {  get; private set; }

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

        // State Machine
        private StateMachine _stateMachine;

        // AI States
        // TODO Make Builder for AI Archetypes and AI configuration
        private AIIdleState _idleState;
        private AIPatrolState _patrolState;
        private AIChaseState _chaseState;
        private AIAttackState _attackState;

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
            if (Output==null)
            {
                Output = new AIInputReader();
            }

            // Configure NavMeshAgent for pathfinding only (no position/rotation update)
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.acceleration = 9999;
            navMeshAgent.angularSpeed = 9999;
            navMeshAgent.speed = 0;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoTraverseOffMeshLink = false;
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
            _stateMachine.SetState(_idleState);
        }

        /// <summary>
        /// Configures all AI behavior state transitions
        /// </summary>
        private void ConfigureTransitions()
        {
            // ----- Idle State Transitions -----
            AddTransition(_idleState, _patrolState, () =>
                patrolPoints != null && patrolPoints.Length > 0 && _idleState.IsIdleComplete());
            AddTransition(_idleState, _chaseState, () =>
                target != null && IsTargetDetected(target));

            // ----- Patrol State Transitions -----
            AddTransition(_patrolState, _idleState, () =>
                _patrolState.HasReachedPatrolPoint());
            AddTransition(_patrolState, _chaseState, () =>
                target != null && IsTargetDetected(target));

            // ----- Chase State Transitions -----
            AddTransition(_chaseState, _attackState, () =>
                target != null && IsTargetInAttackRange(target));
            AddTransition(_chaseState, _patrolState, () =>
                HasLostTarget(target) && patrolPoints != null && patrolPoints.Length > 0);
            AddTransition(_chaseState, _idleState, () =>
                HasLostTarget(target) && (patrolPoints == null || patrolPoints.Length == 0));

            // ----- Attack State Transitions -----
            AddTransition(_attackState, _chaseState, () =>
                target != null && !IsTargetInAttackRange(target));
            AddTransition(_attackState, _patrolState, () =>
                HasLostTarget(target) && patrolPoints != null && patrolPoints.Length > 0);
            AddTransition(_attackState, _idleState, () =>
                HasLostTarget(target) && (patrolPoints == null || patrolPoints.Length == 0));
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
        /// Checks if target is detected (in range and line of sight)
        /// </summary>
        private bool IsTargetDetected(Transform target)
        {
            if (target == null) return false;

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > detectionRange) return false;

            return HasLineOfSight();
        }

        /// <summary>
        /// Checks if target is in attack range
        /// </summary>
        private bool IsTargetInAttackRange(Transform target)
        {
            return target != null && Vector3.Distance(transform.position, target.position) <= attackRange;
        }

        /// <summary>
        /// Checks if target has been lost
        /// </summary>
        private bool HasLostTarget(Transform target)
        {
            if (target == null) return true;
            return Vector3.Distance(transform.position, target.position) > detectionRange * 1.5f;
        }

        /// <summary>
        /// Gets distance to target
        /// </summary>
        public float GetDistanceToTarget()
        {
            return target == null ? float.MaxValue : Vector3.Distance(transform.position, target.position);
        }

        /// <summary>
        /// Gets direction to target
        /// </summary>
        public Vector3 GetDirectionToTarget()
        {
            if (target == null) return transform.forward;

            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            return direction.normalized;
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_stateMachine == null) return;
            currentStateName = _stateMachine.CurrentNode?.State?.GetType().Name ?? "None";
            _stateMachine.Update();
            // Get desired velocity from NavMeshAgent and convert to movement input
            // This allows NavMeshAgent to calculate direction while Character handles actual movement
            if (navMeshAgent != null && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                Vector3 desiredDirection = navMeshAgent.path.corners[1] - transform.position;
                desiredDirection.y = 0;
                Output.SetMovementInput(new Vector2(desiredDirection.x,desiredDirection.z).normalized);

            }
            else
            {
                Output.SetMovementInput(Vector2.zero);
            }
        }
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            if (_stateMachine == null) return;
            _stateMachine.FixedUpdate();


        }


        /// <summary>
        /// Sets destination for NavMeshAgent
        /// </summary>
        public void SetDestination(Vector3 destination)
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.destination != destination)
            {
                navMeshAgent.SetDestination(destination);
            }
        }

        /// <summary>
        /// Clears destination (resets desired velocity to zero)
        /// </summary>
        public void ClearDestination()
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
            }
        }

        /// <summary>
        /// Sets a new target
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
            if (target != null && IsTargetInAttackRange(target))
            {
                _stateMachine.ChangeState(_attackState);
            }
            else
            {
                Debug.LogWarning("Cannot go to Attack state - target not in range");
            }
        }

        /// <summary>
        /// Gets the current state name
        /// </summary>
        public string GetCurrentStateName()
        {
            return currentStateName;
        }

        /// <summary>
        /// Draws gizmos for AI visualization
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
