using LOGIYGames.Movement;
using UnityEngine;

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

        [Header("Detection Settings")]
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float lostTargetTimeout = 5f;

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 2f;

        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        private string currentStateName;

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
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public StateMachine StateMachine => _stateMachine;

        private void Awake()
        {

            if (aiInput == null)
            {
                Debug.LogError("AIBrain requires AIInputReader component");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine();

            // Initialize all states
            _idleState = new AIIdleState(this);
            _patrolState = new AIPatrolState(this, patrolWaitTime);
            _chaseState = new AIChaseState(this, lostTargetTimeout);
            _attackState = new AIAttackState(this);

            // Configure all transitions
            ConfigureTransitions();

            // Set initial state
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                _stateMachine.SetState(_patrolState);
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
            // Idle           | Patrol        | Has patrol points & idle timeout
            // Idle           | Chase         | Target detected
            // --------------------------------------------
            // Patrol         | Idle          | No patrol points or manual switch
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
                patrolPoints != null && patrolPoints.Length > 0 && _idleState.GetIdleDuration() > 0);
            AddTransition(_idleState, _chaseState, () => 
                target != null && IsTargetDetected());

            // ----- Patrol State Transitions -----
            AddTransition(_patrolState, _idleState, () => 
                patrolPoints == null || patrolPoints.Length == 0);
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
        /// Draws gizmos for AI visualization in editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
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
