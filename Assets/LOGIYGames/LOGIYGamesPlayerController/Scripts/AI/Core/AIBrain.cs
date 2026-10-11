using LOGIYGames.CharacterCore;
using LOGIYGames.Scripts.AI;
using Unity.AI.Navigation;
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
        public bool JumpPressed { get; set; }
        public bool EvadePressed { get; set; }
        public bool CrouchPressed { get; set; }
        public bool SprintPressed { get; set; }
        public bool FocusPressed { get; set; }


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

        public TargetingController TargetingController;
        Actor Actor;
        private void Awake()
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }


            InputReader = new(this);
            Actor = GetComponent<Actor>();
            Actor.DefaultMovementStrategy = new CharacterForwardMovement(Actor);
            Actor.DefaultRotationStrategy = new CharacterRelativeRotation(Actor);
            if (Actor.Motor is not NavMeshControllerWrapper)
            {
                ConfigureNavMesh();
            }
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
            _stateMachine = new StateMachine();
            statesPreset.Init(this);
        }

        public bool HasLineOfSight()
        {
            if (TargetingController.CurrentTarget == null) return false;

            Vector3 direction = TargetingController.CurrentTarget.position - transform.position;
            float distance = direction.magnitude;

            if (distance > detectionRange) return false;

            direction.Normalize();

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out RaycastHit hit, distance))
            {
                return hit.transform == TargetingController.CurrentTarget || hit.transform.IsChildOf(TargetingController.CurrentTarget);
            }

            return true;
        }

        public bool IsTargetDetected()
        {
            if (TargetingController.CurrentTarget == null) return false;

            float distance = Vector3.Distance(transform.position, TargetingController.CurrentTarget.position);
            if (distance > detectionRange) return false;

            return HasLineOfSight();
        }


        public bool HasLostTarget()
        {
            if (TargetingController.CurrentTarget == null) return true;
            return Vector3.Distance(transform.position, TargetingController.CurrentTarget.position) > detectionRange * 1.5f;
        }

        public float GetDistanceToTarget()
        {
            return TargetingController.CurrentTarget == null ? float.MaxValue : Vector3.Distance(transform.position, TargetingController.CurrentTarget.position);
        }

        public Vector3 GetDirectionToTarget()
        {
            if (TargetingController.CurrentTarget == null) return transform.forward;

            Vector3 direction = TargetingController.CurrentTarget.position - transform.position;
            direction.y = 0;
            return direction.normalized;
        }
        public override void OnUpdate(float deltaTime)
        {

            base.OnUpdate(deltaTime);
            if (_stateMachine == null) return;

            CrouchPressed = false;
            JumpPressed = false;
            EvadePressed = false;
            CrouchPressed = false;
            SprintPressed = false;
            FocusPressed = false;

   

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
                    JumpPressed = true;

                    navMeshAgent.speed = 2f;
                    navMeshAgent.transform.rotation = link.endTransform.rotation;
                }
                if (link.area == NavMesh.GetAreaFromName("Roll"))
                {
                    CrouchPressed = true;

                    navMeshAgent.speed = 2f;
                    navMeshAgent.transform.rotation = link.endTransform.rotation;
                }
            }
            else
            {
                navMeshAgent.speed = Actor.RuntimeMovement.CurrentSpeed * 3.5f;
            }

            currentStateName = _stateMachine.CurrentNode?.State?.GetType().Name ?? "None";
            FocusPressed = true ? TargetingController.CurrentTarget != null : false;
            _stateMachine.Update();
            if (navMeshAgent != null && navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                Vector3 desiredDirection = transform.InverseTransformDirection(navMeshAgent.path.corners[1] - transform.position);
                desiredDirection.y = 0;
                MovementInput = new Vector2(desiredDirection.x, desiredDirection.z).normalized;

            }
            else
            {
                MovementInput = Vector2.zero;
            }
            Actor.UpdateInput(InputReader.GetInput());
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
