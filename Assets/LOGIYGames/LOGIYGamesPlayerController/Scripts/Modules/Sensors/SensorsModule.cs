using UnityEngine;
namespace LOGIYGames
{
    [RequireComponent(typeof(CharacterController))]
    public class SensorsModule : MonoModuleBase
    {

        [Header("Component References")]
        [SerializeField] private CharacterController characterController;

        [Header("Detection Settings")]
        [SerializeField] private float rayDistance = 0.3f;
        [SerializeField] private float forwardDownRayDistance = 0.3f;
        [SerializeField] private float upCheckDistance = 1f;
        [SerializeField] private float groundCheckDistance = 0.27f;
        [SerializeField] private float belowCheckDistance = 0.27f;
        [SerializeField] private float castUpSphereRadius = 0.3f;
        [SerializeField] private float castDownSphereRadius = 0.3f;        
        [SerializeField] private float foreheadRaysInterval = 0.2f; // Расстояние между лучами по вертикали
        [SerializeField] private float detectionOriginYOffset = 0.5f; 


        [Header("Layer Masks")]
        [SerializeField] private LayerMask includeLayers;
        [SerializeField] private LayerMask groundLayers;

        [Header("Debug Info")]
        [SerializeField] private bool showDebugInfo = true;
        private string legsLeftObstacleName;
        private string legsRightObstacleName;
        private string legsFrontObstacleName;
        private string legsFrontDownObstacleName;
        private string headFrontObstacleName;
        private string headAboveFrontObstacleName;
        private string headRightFrontObstacleName;
        private string headLeftFrontObstacleName;
        private string aboveObstacleName;
        private string belowObstacleName;


        public Vector3 headFrontOrigin => new Vector3(
                characterController.bounds.center.x,
                characterController.bounds.max.y,
                characterController.bounds.center.z);
        public Vector3 headLeftFrontOrigin => new Vector3(
        characterController.bounds.center.x,
        characterController.bounds.max.y,
        characterController.bounds.center.z) - characterController.transform.right * characterController.radius * 2;
        public Vector3 headRightFrontOrigin => new Vector3(
        characterController.bounds.center.x,
        characterController.bounds.max.y,
        characterController.bounds.center.z) + characterController.transform.right * characterController.radius * 2;
        public Vector3 headAboveFrontOrigin => new Vector3(
        characterController.bounds.center.x,
        characterController.bounds.max.y,
        characterController.bounds.center.z) + characterController.transform.up * foreheadRaysInterval;
        public Vector3 detectionOrigin => new Vector3(
                characterController.bounds.center.x,
                characterController.bounds.min.y + detectionOriginYOffset,
                characterController.bounds.center.z);

        // Detection Results
        private RaycastHit belowHit;
        private RaycastHit groundHit;
        private RaycastHit aboveHit;
        private RaycastHit headFrontHit;
        private RaycastHit headRightFrontHit;
        private RaycastHit headLeftFrontHit;
        private RaycastHit headAboveFrontHit;
        private RaycastHit legsFrontHit;
        private RaycastHit kneesFrontHit;
        private RaycastHit kneesFrontDownHit;
        private RaycastHit legsRightHit;
        private RaycastHit legsLeftHit;

        // Public Properties
        public RaycastHit BelowHit => belowHit;
        public RaycastHit GroundHit => groundHit;
        public RaycastHit AboveHit => aboveHit;
        public RaycastHit ForeheadFrontHit => headFrontHit;
        public RaycastHit ForeheadLeftFrontHit => headLeftFrontHit;
        public RaycastHit ForeheadRightFrontHit => headRightFrontHit;
        public RaycastHit ForeheadAboveFrontHit => headAboveFrontHit;
        public RaycastHit LegsFrontHit => legsFrontHit;
        public RaycastHit KneesFrontHit => kneesFrontHit;
        public RaycastHit LegsFrontDownHit => legsFrontHit;
        public RaycastHit LegsRightHit => legsRightHit;
        public RaycastHit LegsLeftHit => legsLeftHit;

        public bool IsObstacleBelow { get; private set; }
        public bool IsObstacleLegsLeft { get; private set; }
        public bool IsObstacleLegsRight { get; private set; }
        public bool IsObstacleLegsFront { get; private set; }
        public bool IsObstacleKneesFront { get; private set; }
        public bool IsObstacleKneesFrontDown { get; private set; }
        public bool IsObstacleHeadFront { get; private set; }
        public bool IsObstacleRightHeadFront { get; private set; }
        public bool IsObstacleLeftHeadFront { get; private set; }
        public bool IsObstacleAboveHeadFront { get; private set; }
        public bool IsObstacleAbove { get; private set; }
        public bool IsOnEdge { get; private set; }
        public bool IsOnSlope { get; private set; }
        public bool IsGrounded {  get; private set; }
        public bool IsStepUpAhead {  get; private set; }
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
        private void Start()
        {
            castDownSphereRadius = characterController.radius - 0.01f;
            castUpSphereRadius = characterController.radius + 0.01f;
            groundCheckDistance = characterController.stepOffset;
            upCheckDistance = characterController.height- castUpSphereRadius;
            belowCheckDistance = 0.1f;
            rayDistance = characterController.radius*2.5f;
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
            // Wall detection
            IsObstacleLegsRight = Physics.Raycast(detectionOrigin, characterController.transform.right, out legsRightHit, rayDistance, includeLayers);
            IsObstacleLegsLeft = Physics.Raycast(detectionOrigin, -characterController.transform.right, out legsLeftHit, rayDistance, includeLayers);
            IsObstacleLegsFront = Physics.Raycast(detectionOrigin, characterController.transform.forward, out legsFrontHit, rayDistance, includeLayers);
            IsObstacleKneesFront = Physics.Raycast(characterController.transform.position+ characterController.stepOffset* characterController.transform.up, characterController.transform.forward, out kneesFrontHit, rayDistance, includeLayers);
            IsObstacleKneesFrontDown = Physics.Raycast(characterController.transform.position+ characterController.stepOffset* characterController.transform.up + characterController.transform.forward * rayDistance, -characterController.transform.up, out kneesFrontDownHit, forwardDownRayDistance, includeLayers);
            IsObstacleHeadFront = Physics.Raycast(headFrontOrigin, characterController.transform.forward, out headFrontHit, rayDistance, includeLayers);
            IsObstacleRightHeadFront = Physics.Raycast(headRightFrontOrigin, characterController.transform.forward, out headRightFrontHit, rayDistance, includeLayers);
            IsObstacleLeftHeadFront = Physics.Raycast(headLeftFrontOrigin, characterController.transform.forward, out headLeftFrontHit, rayDistance, includeLayers);
            IsObstacleAboveHeadFront = Physics.Raycast(headAboveFrontOrigin, characterController.transform.forward, out headAboveFrontHit, rayDistance, includeLayers);
            IsObstacleAbove = Physics.SphereCast(detectionOrigin, castUpSphereRadius, characterController.transform.up, out aboveHit, upCheckDistance, includeLayers);
            IsObstacleBelow = Physics.SphereCast(detectionOrigin+characterController.transform.up* castDownSphereRadius, castDownSphereRadius, -characterController.transform.up, out belowHit, belowCheckDistance, includeLayers);
            
            IsGrounded = Physics.SphereCast(detectionOrigin + characterController.transform.up * castDownSphereRadius, castDownSphereRadius, -characterController.transform.up,out groundHit, groundCheckDistance, groundLayers);
             

            GroundAngle = Vector3.SignedAngle(characterController.transform.up, belowHit.normal, characterController.transform.right);
            IsOnSlope = GroundAngle == 0 ? false : true;
            IsStepUpAhead = IsObstacleKneesFrontDown
                && IsObstacleBelow
                && (kneesFrontDownHit.point.y - characterController.transform.position.y)<=characterController.stepOffset-detectionOriginYOffset
                && (kneesFrontDownHit.point.y - characterController.transform.position.y)>=0.2f
                && Mathf.Abs( Vector3.Angle(characterController.transform.up, kneesFrontDownHit.normal)) <= MaxStableSlopeAngle;

            print(kneesFrontDownHit.point.y - characterController.transform.position.y);
            print(characterController.stepOffset - detectionOriginYOffset);
            EdgeDetection();
        }

        private void EdgeDetection()
        {
            IsOnEdge = IsObstacleBelow && !Physics.Raycast(characterController.transform.position, -characterController.transform.up, rayDistance, includeLayers);
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
            legsFrontDownObstacleName = kneesFrontDownHit.transform?.name;
            headFrontObstacleName = ForeheadFrontHit.transform?.name;
            headAboveFrontObstacleName = ForeheadAboveFrontHit.transform?.name;
            headRightFrontObstacleName = ForeheadRightFrontHit.transform?.name;
            headLeftFrontObstacleName = ForeheadLeftFrontHit.transform?.name;
            aboveObstacleName = AboveHit.transform?.name;
            belowObstacleName = BelowHit.transform?.name;
        }

        private void OnDrawGizmos()
        {
            if (characterController == null) return;

            if (!showDebugInfo) return;
            // Draw rays
            DrawDetectionRays(detectionOrigin);

            // Draw sphere casts
            DrawSphereCasts(detectionOrigin);
            DrawBelowPlane();
        }
        private void DrawBelowPlane()
        {
            DebugDraw.DrawPlane(belowHit.point,belowHit.normal,1, Color.green);
        }
        private void DrawDetectionRays(Vector3 origin)
        {

            Debug.DrawRay(origin, -characterController.transform.right * rayDistance, IsObstacleLegsLeft ? Color.green : Color.red, 0, false);
            Debug.DrawRay(origin, characterController.transform.right * rayDistance, IsObstacleLegsRight ? Color.green : Color.red, 0, false);
            Debug.DrawRay(origin, characterController.transform.forward * rayDistance, IsObstacleLegsFront ? Color.green : Color.red, 0, false);
            Debug.DrawRay(characterController.transform.position+characterController.stepOffset*characterController.transform.up, characterController.transform.forward * rayDistance, IsObstacleKneesFront ? Color.green : Color.red, 0, false);
            
            Debug.DrawRay(characterController.transform.position + characterController.stepOffset * characterController.transform.up + characterController.transform.forward * rayDistance, -characterController.transform.up * forwardDownRayDistance, IsObstacleKneesFrontDown ? Color.green : Color.red, 0, false);

            Debug.DrawRay(headLeftFrontOrigin, characterController.transform.forward * rayDistance, IsObstacleLeftHeadFront ? Color.green : Color.red, 0, false);
            Debug.DrawRay(headRightFrontOrigin, characterController.transform.forward * rayDistance, IsObstacleRightHeadFront ? Color.green : Color.red, 0, false);
            Debug.DrawRay(headFrontOrigin, characterController.transform.forward * rayDistance, IsObstacleHeadFront ? Color.green : Color.red, 0, false);
            Debug.DrawRay(headAboveFrontOrigin, characterController.transform.forward * rayDistance, IsObstacleAboveHeadFront ? Color.green : Color.red, 0, false);
        }

        private void DrawSphereCasts(Vector3 origin)
        {

            Gizmos.color = IsObstacleAbove ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + characterController.transform.up * upCheckDistance, castUpSphereRadius);

            Gizmos.color = IsObstacleBelow ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + characterController.transform.up * castDownSphereRadius + -characterController.transform.up * belowCheckDistance, castDownSphereRadius);

            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(origin + characterController.transform.up * castDownSphereRadius + -characterController.transform.up * groundCheckDistance, castDownSphereRadius);

            if (IsObstacleBelow)
            {

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(belowHit.point + characterController.transform.up * castDownSphereRadius, castDownSphereRadius);
            }

        }
    }
}