using UnityEngine;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    [RequireComponent(typeof(CharacterController))]
    public class SensorsModule : MonoModuleBase
    {
        [Header("Component References")]
        [SerializeField] private CharacterController characterController;

        [Header("Detection Settings")]
        [SerializeField] private float rayDistance = 0.3f;
        [SerializeField] private float upCheckDistance = 1f;
        [SerializeField] private float downCheckDistance = 0.27f;
        [SerializeField] private float castUpSphereRadius = 0.3f;
        [SerializeField] private float castDownSphereRadius = 0.3f;

        [Header("Layer Mask")]
        [SerializeField] private LayerMask includeLayers;

        [Header("Debug Info")]
        [SerializeField] private bool showDebugInfo = true;
        private string legsLeftObstacleName;
        private string legsRightObstacleName;
        private string legsFrontObstacleName;
        private string headFrontObstacleName;
        private string aboveObstacleName;
        private string belowObstacleName;


        public Vector3 headFrontOrigin => new Vector3(
                characterController.bounds.center.x,
                characterController.bounds.max.y,
                characterController.bounds.center.z);

        public Vector3 detectionOrigin => new Vector3(
                characterController.bounds.center.x,
                characterController.bounds.min.y + 0.5f,
                characterController.bounds.center.z);

        // Detection Results
        private RaycastHit belowHit;
        private RaycastHit aboveHit;
        private RaycastHit headFrontHit;
        private RaycastHit legsFrontHit;
        private RaycastHit legsRightHit;
        private RaycastHit legsLeftHit;

        // Public Properties
        public RaycastHit BelowHit => belowHit;
        public RaycastHit AboveHit => aboveHit;
        public RaycastHit ForeheadFrontHit => headFrontHit;
        public RaycastHit LegsFrontHit => legsFrontHit;
        public RaycastHit LegsRightHit => legsRightHit;
        public RaycastHit LegsLeftHit => legsLeftHit;

        public bool IsObstacleBelow { get; private set; }
        public bool IsObstacleLegsLeft { get; private set; }
        public bool IsObstacleLegsRight { get; private set; }
        public bool IsObstacleLegsFront { get; private set; }
        public bool IsObstcleHeadFront { get; private set; }
        public bool IsObstacleAbove { get; private set; }
        public bool IsOnEdge { get; private set; }
        public bool IsOnSlope { get; private set; }
        public float GroundAngle { get; private set; }

        public bool IsGrounded
        {
            get => IsObstacleBelow && IsValidSlope(BelowHit.normal);
        }
        float prevGroundAngle;

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            PerformDetection();
        }
        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            if (showDebugInfo)
            {
                UpdateDebugInfo();
            }
        }
        public bool IsValidSlope(Vector3 normal)
        {
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = Mathf.Abs( angle) <= 70;
            return validAngle;
        }
        private void PerformDetection()
        {
            // Wall detection
            IsObstacleLegsRight = Physics.Raycast(detectionOrigin, characterController.transform.right, out legsRightHit, rayDistance, includeLayers);
            IsObstacleLegsLeft = Physics.Raycast(detectionOrigin, -characterController.transform.right, out legsLeftHit, rayDistance, includeLayers);
            IsObstacleLegsFront = Physics.Raycast(detectionOrigin, characterController.transform.forward, out legsFrontHit, rayDistance, includeLayers);

            IsObstcleHeadFront = Physics.Raycast(
                headFrontOrigin,
                characterController.transform.forward,
                out headFrontHit,
                rayDistance,
                includeLayers);

            // Above and ground detection
            IsObstacleAbove = Physics.SphereCast(
                detectionOrigin,
                castUpSphereRadius,
                Vector3.up,
                out aboveHit,
                upCheckDistance,
                includeLayers);

            IsObstacleBelow = Physics.SphereCast(
                detectionOrigin,
                castDownSphereRadius,
                Vector3.down,
                out belowHit,
                downCheckDistance,
                includeLayers);

            GroundAngle = Vector3.SignedAngle(characterController.transform.up, belowHit.normal, characterController.transform.right);
            IsOnSlope = GroundAngle == 0 ? false : true;

            if (EdgeDetection())
            {
                IsOnEdge = true;
            }
            else
            {
                IsOnEdge = false;
            }


            prevGroundAngle = GroundAngle;

        }

        private bool EdgeDetection()
        {
            return IsObstacleBelow && !Physics.Raycast(characterController.transform.position, -characterController.transform.up, rayDistance, includeLayers);
        }
        private ref RaycastHit GetHitForDirection(Vector3 direction)
        {
            if (direction == characterController.transform.right) return ref legsRightHit;
            if (direction == -characterController.transform.right) return ref legsLeftHit;
            return ref legsFrontHit;
        }

        private void UpdateDebugInfo()
        {
            legsLeftObstacleName = LegsLeftHit.transform?.name;
            legsRightObstacleName = LegsRightHit.transform?.name;
            legsFrontObstacleName = LegsFrontHit.transform?.name;
            headFrontObstacleName = ForeheadFrontHit.transform?.name;
            aboveObstacleName = AboveHit.transform?.name;
            belowObstacleName = BelowHit.transform?.name;
        }

        private void OnDrawGizmos()
        {
            if (characterController == null) return;


            // Draw rays
            DrawDetectionRays(detectionOrigin);

            // Draw sphere casts
            DrawSphereCasts(detectionOrigin);
        }

        private void DrawDetectionRays(Vector3 origin)
        {

            Debug.DrawRay(origin, -characterController.transform.right * rayDistance, IsObstacleLegsLeft ? Color.green : Color.red, 0, false);
            Debug.DrawRay(origin, characterController.transform.right * rayDistance, IsObstacleLegsRight ? Color.green : Color.red, 0, false);
            Debug.DrawRay(origin, characterController.transform.forward * rayDistance, IsObstacleLegsFront ? Color.green : Color.red, 0, false);



            Debug.DrawRay(headFrontOrigin, characterController.transform.forward * rayDistance, IsObstcleHeadFront ? Color.green : Color.red, 0, false);
        }

        private void DrawSphereCasts(Vector3 origin)
        {

            Gizmos.color = IsObstacleAbove ? Color.green : Color.red;

            Gizmos.DrawWireSphere(origin + Vector3.up * upCheckDistance, castUpSphereRadius);

            Gizmos.color = IsObstacleBelow ? Color.green : Color.red;

            Gizmos.DrawWireSphere(origin + Vector3.down * downCheckDistance, castDownSphereRadius);
        }
    }
}