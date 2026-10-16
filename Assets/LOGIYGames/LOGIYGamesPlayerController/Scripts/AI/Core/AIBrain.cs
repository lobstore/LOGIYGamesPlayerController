using LOGIYGames.CharacterCore;
using LOGIYGames.Scripts.AI;
using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace LOGIYGames.AI
{
    public class AIBrain : MonoModuleBase
    {
        private ICharacterInputReader InputReader;
        public CharacterInput Input;
        [Header("References")]

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private AIStatesBuilder statesPreset;

        [Header("Debug")]
        [SerializeField] private bool debugDraw = true;

        private string currentStateName;
        private StateMachine m_stateMachine;
        public StateMachine StateMachine => m_stateMachine;
        Actor Actor;

        [Header("AI Perception")]
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private float sightRange = 10f;

        [SerializeField, Range(0f, 360f)]
        private float viewAngle = 180f;

        [SerializeField] private float targetSearchInterval = 0.25f;

        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 1f;

        [Header("Combat")]
        [SerializeField] private float attackRange = 1.7f;
        [SerializeField] private float attackCooldown = 1.2f;
        [SerializeField] private float attackHitRangeBonus = 0.25f;

        private float _nextTargetSearchTime;


        [Header("Combat Movement")]
        [SerializeField] private float combatEnterRange = 5f;
        [SerializeField] private float combatExitRange = 7f;

        [SerializeField] private float preferredCombatDistance = 1.55f;
        [SerializeField] private float minCombatDistance = 0.9f;

        [SerializeField] private float combatRepathInterval = 0.35f;
        [Header("Combat Attack")]
        [SerializeField, Range(0f, 180f)]
        private float attackAngle = 65f;

        [SerializeField] private float attackCommitTime = 0.65f;



        public float CombatEnterRange => combatEnterRange;

        public float CombatExitRange =>
            Mathf.Max(combatExitRange, combatEnterRange);

        public float PreferredCombatDistance => preferredCombatDistance;

        public float MinCombatDistance => minCombatDistance;

        public float CombatRepathInterval =>
            Mathf.Max(0.1f, combatRepathInterval);
        public float AttackAngle => attackAngle;

        public float AttackCommitTime => attackCommitTime;

        public float DistanceToTarget
        {
            get
            {
                if (target == null)
                    return float.PositiveInfinity;

                Vector3 difference = target.position - transform.position;
                difference.y = 0f;

                return difference.magnitude;
            }
        }


        private void Awake()
        {
            InputReader = new AIInputReader(this);
            navMeshAgent = GetComponent<NavMeshAgent>();
            Actor = GetComponent<Actor>();
            Actor.SetInput(InputReader);
            Actor.OnDamageTaken.AddListener(agressor => { });
            if (Actor.Motor is not NavMeshControllerWrapper) ConfigureNavMesh();
            InitializeStateMachine();
        }

        private void ConfigureNavMesh()
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.acceleration = 0;
            navMeshAgent.angularSpeed = 0;
            navMeshAgent.speed = 0;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoTraverseOffMeshLink = false;
        }

        private void InitializeStateMachine()
        {
            m_stateMachine = new StateMachine();
            statesPreset.Build(this);
        }
        public override void OnUpdate(float deltaTime)
        {

            base.OnUpdate(deltaTime);
            if (m_stateMachine == null) return;
            Input = new();
            Input.LookForward = transform.forward;
            Input.LookRight = transform.right;
            RefreshTarget();
            var link = navMeshAgent.currentOffMeshLinkData.owner as NavMeshLink;
            if (link != null && navMeshAgent.isOnOffMeshLink)
            {

                if (link.area == NavMesh.GetAreaFromName("Climbable"))
                {
                    navMeshAgent.speed = 0.3f;
                    navMeshAgent.transform.rotation = link.endTransform.rotation;
                }
                if (link.area == NavMesh.GetAreaFromName("Jump"))
                {
                    Input.AttackPressed = true;

                    navMeshAgent.speed = 2f;
                    navMeshAgent.transform.rotation = link.endTransform.rotation;
                }
                if (link.area == NavMesh.GetAreaFromName("Roll"))
                {
                    Input.MovementInput = Vector2.up;
                    Input.CrouchPressed = true;

                    navMeshAgent.speed = 2f;
                    navMeshAgent.transform.rotation = link.endTransform.rotation;
                }
            }
            else
            {
                navMeshAgent.speed = Actor.RuntimeMovement.CurrentSpeed * 3.5f;
            }

            currentStateName = m_stateMachine.CurrentNode?.State?.GetType().Name ?? "None";
            if (navMeshAgent != null && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                Vector3 desiredDirection = transform.InverseTransformDirection(navMeshAgent.path.corners[1] - transform.position);
                desiredDirection.y = 0;
                Input.MovementInput = new Vector2(desiredDirection.x, desiredDirection.z).normalized;

            }
            else
            {
                Input.MovementInput = Vector2.zero;
            }
            m_stateMachine.Update();

        }
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            if (m_stateMachine == null) return;
            m_stateMachine.FixedUpdate();


        }

        public NavMeshAgent Agent => navMeshAgent;

        public Transform Target => target;

        public bool HasTarget => target != null;

        public bool HasPatrolPoints =>
            patrolPoints != null &&
            Array.Exists(patrolPoints, point => point != null);

        public float PatrolWaitTime => patrolWaitTime;

        public float AttackCooldown => attackCooldown;

        public float AttackRange => attackRange;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void ClearTarget()
        {
            target = null;
        }

        public bool SetMoveDestination(Vector3 destination)
        {
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
                return false;

            navMeshAgent.isStopped = false;

            return navMeshAgent.SetDestination(destination);
        }

        public void StopMoving()
        {
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
                return;

            navMeshAgent.isStopped = true;

            if (!navMeshAgent.isOnOffMeshLink &&
                navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
            }
        }

        public bool HasReachedDestination(float extraDistance = 0.15f)
        {
            if (navMeshAgent == null ||
                !navMeshAgent.isOnNavMesh ||
                navMeshAgent.pathPending ||
                !navMeshAgent.hasPath)
            {
                return false;
            }

            return navMeshAgent.remainingDistance <=
                   navMeshAgent.stoppingDistance + extraDistance;
        }

        public bool IsTargetInAttackRange(float extraRange = 0f)
        {
            if (target == null)
                return false;

            Vector3 difference = target.position - transform.position;
            difference.y = 0f;

            float effectiveRange = attackRange + extraRange;

            return difference.sqrMagnitude <=
                   effectiveRange * effectiveRange;
        }


        public Transform GetPatrolPoint(int index)
        {
            if (patrolPoints == null ||
                index < 0 ||
                index >= patrolPoints.Length)
            {
                return null;
            }

            return patrolPoints[index];
        }

        public int GetClosestPatrolPointIndex()
        {
            if (!HasPatrolPoints)
                return -1;

            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Transform point = patrolPoints[i];

                if (point == null)
                    continue;

                float distance =
                    (point.position - transform.position).sqrMagnitude;

                if (distance >= closestDistance)
                    continue;

                closestDistance = distance;
                closestIndex = i;
            }

            return closestIndex;
        }

        public int GetNextPatrolPointIndex(int currentIndex)
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
                return -1;

            int count = patrolPoints.Length;

            for (int offset = 1; offset <= count; offset++)
            {
                int index = (currentIndex + offset + count) % count;

                if (patrolPoints[index] != null)
                    return index;
            }

            return -1;
        }


        private void RefreshTarget()
        {
            // Сохраняем текущую цель, пока она существует.
            if (target != null)
                return;

            if (Time.time < _nextTargetSearchTime)
                return;

            _nextTargetSearchTime =
                Time.time + Mathf.Max(0.05f, targetSearchInterval);

            target = FindNearestTarget();
        }

        private Transform FindNearestTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(
                transform.position,
                sightRange,
                targetLayers,
                QueryTriggerInteraction.Collide);

            Transform nearestTarget = null;
            float closestDistance = float.MaxValue;

            Vector3 forward = transform.forward;
            forward.y = 0f;

            foreach (Collider hit in colliders)
            {
                Actor candidateActor =
                    hit.GetComponentInParent<Actor>();

                if (candidateActor == null)
                    continue;

                // Не обнаруживаем самого себя или коллайдеры своих детей.
                if (candidateActor == Actor ||
                    candidateActor.transform.root == transform.root || candidateActor.transform.tag == "Enemy")
                {
                    continue;
                }

                Transform candidate = candidateActor.transform;

                Vector3 direction =
                    candidate.position - transform.position;

                direction.y = 0f;

                float distance = direction.sqrMagnitude;

                if (distance >= closestDistance)
                    continue;

                if (viewAngle < 360f &&
                    forward.sqrMagnitude > 0.0001f &&
                    Vector3.Angle(forward, direction) > viewAngle * 0.5f)
                {
                    continue;
                }

                closestDistance = distance;
                nearestTarget = candidate;
            }

            return nearestTarget;
        }

        public void RequestAttack(Transform victim)
        {
            if (victim == null)
                return;

            Input.AttackPressed = true;
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

        }
    }
}
