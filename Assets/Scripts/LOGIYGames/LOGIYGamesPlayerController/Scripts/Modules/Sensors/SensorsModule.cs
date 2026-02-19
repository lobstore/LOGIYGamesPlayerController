using UnityEngine;
namespace LOGIYGames
{
    [RequireComponent(typeof(CharacterController))]
    public class SensorsModule : MonoModuleBase
    {

        [Header("Component References")]
        [SerializeField] private CharacterController characterController;

        [Header("Detection Settings")]
        [SerializeField] private float upCheckDistance = 0.8f;
        [SerializeField] private float groundCheckDistance = 0.8f;
        [SerializeField] private float belowCheckDistance = 0.8f;
        [SerializeField] private float castUpSphereRadius = 0.2f;
        [SerializeField] private float castDownSphereRadius = 0.2f;        
        [SerializeField] private float detectionOriginYOffset = 0f; 


        [Header("Layer Masks")]
        [SerializeField] private LayerMask includeLayers;
        [SerializeField] private LayerMask groundLayers;

        [Header("Debug Info")]
        [SerializeField] private bool showDebugInfo = true;
        private string aboveObstacleName;
        private string belowObstacleName;

        public Vector3 detectionOrigin => new Vector3(
                characterController.bounds.center.x,
                characterController.bounds.center.y + detectionOriginYOffset,
                characterController.bounds.center.z);

        // Detection Results
        private RaycastHit belowHit;
        private RaycastHit groundHit;
        private RaycastHit aboveHit;

        // Public Properties
        public RaycastHit BelowHit => belowHit;
        public RaycastHit GroundHit => groundHit;
        public RaycastHit AboveHit => aboveHit;

        public bool IsObstacleBelow { get; private set; }
        public bool IsObstacleAbove { get; private set; }
        public bool IsOnSlope { get; private set; }
        public bool IsGrounded {  get; private set; }
        public float GroundAngle { get; private set; }

        [Range(0, 90)]
        public float MaxStableSlopeAngle;

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
        public bool IsValidSlope()
        {
            float angle = Vector3.Angle(BelowHit.normal, characterController.transform.up);
            bool validAngle = Mathf.Abs(angle) <= MaxStableSlopeAngle;
            return validAngle;
        }
        private void PerformDetection()
        {
            AboveObstaclesDetection();
            BelowObstaclesDetection();

            IsGrounded = Physics.SphereCast(detectionOrigin, castDownSphereRadius, -characterController.transform.up, out groundHit, groundCheckDistance, groundLayers);


            GroundAngle = Vector3.SignedAngle(characterController.transform.up, belowHit.normal, characterController.transform.right);
            IsOnSlope = GroundAngle == 0 ? false : true;

        }

        private void BelowObstaclesDetection()
        {
            IsObstacleBelow = Physics.SphereCast(detectionOrigin, castDownSphereRadius, -characterController.transform.up, out belowHit, belowCheckDistance, includeLayers);
        }

        private void AboveObstaclesDetection()
        {
            IsObstacleAbove = Physics.SphereCast(detectionOrigin, castUpSphereRadius, characterController.transform.up, out aboveHit, upCheckDistance, includeLayers);
        }

        private void UpdateDebugInfo()
        {
            aboveObstacleName = AboveHit.transform?.name;
            belowObstacleName = BelowHit.transform?.name;
        }

        private void OnDrawGizmos()
        {
            if (characterController == null) return;

            if (!showDebugInfo) return;

            // Draw sphere casts
            DrawSphereCasts(detectionOrigin);
            DrawBelowPlane();
        }
        private void DrawBelowPlane()
        {
            DebugDraw.DrawPlane(belowHit.point,belowHit.normal,1, Color.green);
        }

        private void DrawSphereCasts(Vector3 origin)
        {

            Gizmos.color = IsObstacleAbove ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + characterController.transform.up * upCheckDistance, castUpSphereRadius);

            Gizmos.color = IsObstacleBelow ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + -characterController.transform.up * belowCheckDistance, castDownSphereRadius);

            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(origin + -characterController.transform.up * groundCheckDistance, castDownSphereRadius);

            if (IsObstacleBelow)
            {

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(belowHit.point, castDownSphereRadius);
            }

        }
    }
}