using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Handles sensor detection for characters using either Unity CharacterController or KinematicCharacterController.
    /// Works with GenericControllerWrapper for seamless controller swapping.
    /// </summary>
    public class SensorsModule : MonoModuleBase
    {

        [Header("Detection Settings")]
        [SerializeField] private float m_upCheckDistance = 0.8f;
        [SerializeField] private float m_groundCheckDistance = 0.8f;
        [SerializeField] private float m_belowCheckDistance = 0.8f;
        [SerializeField] private float m_castUpSphereRadius = 0.2f;
        [SerializeField] private float m_castDownSphereRadius = 0.2f;
        [SerializeField] private float m_detectionOriginYOffset = 0f;


        [Header("Layer Masks")]
        [SerializeField] private LayerMask m_includeLayers;
        [SerializeField] private LayerMask m_groundLayers;

        [Header("Debug Info")]
        [SerializeField] private bool m_showDebugInfo = true;
        private string m_aboveObstacleName;
        private string m_belowObstacleName;
        private string m_legsForwardObstacleName;

        [SerializeField] Collider col;

        // Detection origin calculated from capsule bounds
        public Vector3 DetectionOrigin
        {
            get
            {
                return new Vector3(
                    col.bounds.center.x,
                    col.bounds.center.y + m_detectionOriginYOffset,
                    col.bounds.center.z
                );
            }
        }

        // Detection Results
        private RaycastHit m_belowHit;
        private RaycastHit m_groundHit;
        private RaycastHit m_aboveHit;
        private RaycastHit m_legsFrontHit;

        // Public Properties
        public RaycastHit BelowHit => m_belowHit;
        public RaycastHit GroundHit => m_groundHit;
        public RaycastHit AboveHit => m_aboveHit;
        public RaycastHit LegsFrontHit => m_legsFrontHit;

        public bool IsObstacleBelow { get; private set; }
        public bool IsObstacleAbove { get; private set; }
        public bool IsOnSlope { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsObstacleLegsFront { get; private set; }
        public bool IsInWater { get; private set; }
        /// <summary>
        /// <value>value</value> > 0: Up,
        /// value < 0: Down, 
        /// value = 0: On plane, or Not grounded
        /// </summary>
        public float GroundAngle { get; private set; }

        [Range(0, 90)]
        public float MaxStableSlopeAngle;

        private void Awake()
        {
            if (col == null)
            {
                col = GetComponent<Collider>();

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
            if (m_showDebugInfo)
            {
                UpdateDebugInfo();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                IsInWater = true;

            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                IsInWater = false;

            }
        }
        public bool IsValidSlope()
        {
            float angle = Vector3.Angle(m_belowHit.normal, transform.up);
            bool validAngle = Mathf.Abs(angle) <= MaxStableSlopeAngle;
            return validAngle;
        }

        private void PerformDetection()
        {
            AboveObstaclesDetection();
            BelowObstaclesDetection();
            LegsFrontObstaclesDetection();
            IsGrounded = Physics.SphereCast(
                DetectionOrigin,
                m_castDownSphereRadius,
                -transform.up,
                out m_groundHit,
                m_groundCheckDistance,
                m_groundLayers
            );

            GroundAngle = Vector3.SignedAngle(

                m_belowHit.normal,
                transform.up,
                transform.right
            );
            IsOnSlope = GroundAngle == 0 ? false : true;
        }

        private void BelowObstaclesDetection()
        {
            IsObstacleBelow = Physics.SphereCast(
                DetectionOrigin,
                m_castDownSphereRadius,
                -transform.up,
                out m_belowHit,
                m_belowCheckDistance,
                m_includeLayers
            );
        }
        private void LegsFrontObstaclesDetection()
        {
            IsObstacleLegsFront = Physics.Raycast(
                DetectionOrigin,
                transform.forward,
                out m_legsFrontHit,
                0.5f,
                m_includeLayers
            );
        }
        private void AboveObstaclesDetection()
        {
            IsObstacleAbove = Physics.SphereCast(
                DetectionOrigin,
                m_castUpSphereRadius,
                transform.up,
                out m_aboveHit,
                m_upCheckDistance,
                m_includeLayers
            );
        }

        private void UpdateDebugInfo()
        {
            m_aboveObstacleName = m_aboveHit.transform?.name;
            m_belowObstacleName = m_belowHit.transform?.name;
            m_legsForwardObstacleName = m_legsFrontHit.transform?.name;
        }

        private void OnDrawGizmos()
        {

            if (!m_showDebugInfo) return;

            // Draw sphere casts
            DrawSphereCasts(DetectionOrigin);
            DrawBelowPlane();

            Debug.DrawRay(DetectionOrigin, transform.forward * 0.5f, Color.red);
        }

        private void DrawBelowPlane()
        {
            DebugDraw.DrawPlane(m_belowHit.point, m_belowHit.normal, 1, Color.green);
        }

        private void DrawSphereCasts(Vector3 origin)
        {
            Gizmos.color = IsObstacleAbove ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + transform.up * m_upCheckDistance, m_castUpSphereRadius);

            Gizmos.color = IsObstacleBelow ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + -transform.up * m_belowCheckDistance, m_castDownSphereRadius);

            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(origin + -transform.up * m_groundCheckDistance, m_castDownSphereRadius);


            if (IsObstacleBelow)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(m_belowHit.point, m_castDownSphereRadius);
            }
        }
    }
}
