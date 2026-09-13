using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{

    public class SensorsModule : MonoModuleBase
    {

        [Header("Detection Settings")]
        [SerializeField] private float m_upCheckDistance = 0.8f;
        [SerializeField] private float m_groundCheckDistance = 0.8f;
        [SerializeField] private float m_belowCheckDistance = 0.8f;
        [SerializeField] private float m_castUpSphereRadius = 0.2f;
        [SerializeField] private float m_castDownSphereRadius = 0.2f;
        [SerializeField] private float m_detectionOriginYOffset = 0f;
        public UnityEvent<bool> GroundedEvent = new();

        [Header("Layer Masks")]
        [SerializeField] private LayerMask m_includeLayers;
        [SerializeField] private LayerMask m_groundLayers;

        [Header("Debug Info")]
        [SerializeField] private bool m_showDebugInfo = true;
        private string m_aboveObstacleName;
        private string m_belowObstacleName;
        private string m_legsRightObstacleName;
        private string m_legsLeftObstacleName;
        private string m_legsForwardObstacleName;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color rayColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color groundPlaneColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color groundedSphereColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color ungroundedSphereColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color aboveObstacleCollidedSphereColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color aboveObstacleNotCollidedSphereColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color belowObstacleCollidedSphereColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color belowObstacleNotCollidedSphereColor;
        [ShowIf(nameof(m_showDebugInfo))][SerializeField] Color belowHitSphereColor;

        [SerializeField] MovementWrapperBase controller;
        Collider col => controller.Collider;

        // Detection origin calculated from capsule bounds
        public Vector3 DetectionOrigin
        {
            get
            {
                return new Vector3(
                    col.bounds.center.x,
                    col.bounds.min.y + m_detectionOriginYOffset,
                    col.bounds.center.z
                );
            }
        }

        // Detection Results
        private RaycastHit m_belowHit;
        private RaycastHit m_groundHit;
        private RaycastHit m_legsLeftHit;
        private RaycastHit m_legsRightHit;
        private RaycastHit m_aboveHit;
        private RaycastHit m_legsFrontHit;

        // Public Properties
        public RaycastHit BelowHit => m_belowHit;
        public RaycastHit GroundHit => m_groundHit;
        public RaycastHit AboveHit => m_aboveHit;
        public RaycastHit LegsFrontHit => m_legsFrontHit;
        public RaycastHit LegsRightHit => m_legsRightHit;
        public RaycastHit LegsLeftHit => m_legsLeftHit;

        public bool IsObstacleBelow { get; private set; }
        public bool IsObstacleAbove { get; private set; }
        public bool IsOnSlope { get; private set; }
        private bool isGrounded;
        public bool IsGrounded
        {
            get
            {
                return isGrounded;
            }
            private set
            {
                if (value != isGrounded)
                {
                    if (value)
                    {
                        GroundedEvent.Invoke(true);
                    }
                    else
                    {
                        GroundedEvent.Invoke(false);
                    }
                }
                isGrounded = value;
            }
        }
        public bool IsObstacleLegsFront { get; private set; }
        public bool IsObstacleLegsRight { get; private set; }
        public bool IsObstacleLegsLeft { get; private set; }
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
            if (controller == null)
                controller = GetComponent<MovementWrapperBase>();
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
            LegsLeftObstaclesDetection();
            LegsRightObstaclesDetection();
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
        private void LegsRightObstaclesDetection()
        {
            IsObstacleLegsRight = Physics.Raycast(
                DetectionOrigin,
                transform.right,
                out m_legsRightHit,
                0.5f,
                m_includeLayers
            );
        }
        private void LegsLeftObstaclesDetection()
        {
            IsObstacleLegsLeft = Physics.Raycast(
                DetectionOrigin,
                -transform.right,
                out m_legsLeftHit,
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

        private void OnDrawGizmosSelected()
        {

            if (!m_showDebugInfo || col == null) return;

            // Draw sphere casts
            DrawSphereCasts(DetectionOrigin);
            DrawBelowPlane();

            Debug.DrawRay(DetectionOrigin, transform.forward * 0.5f, rayColor);
            Debug.DrawRay(DetectionOrigin, transform.right * 0.5f, rayColor);
            Debug.DrawRay(DetectionOrigin, -transform.right * 0.5f, rayColor);
        }

        private void DrawBelowPlane()
        {
            DebugDraw.DrawPlane(m_belowHit.point, m_belowHit.normal, 1, groundPlaneColor);
        }

        private void DrawSphereCasts(Vector3 origin)
        {
            Gizmos.color = IsObstacleAbove ? aboveObstacleCollidedSphereColor : aboveObstacleNotCollidedSphereColor;
            Gizmos.DrawWireSphere(origin + transform.up * m_upCheckDistance, m_castUpSphereRadius);

            Gizmos.color = IsObstacleBelow ? belowObstacleCollidedSphereColor : belowObstacleNotCollidedSphereColor;
            Gizmos.DrawWireSphere(origin + -transform.up * m_belowCheckDistance, m_castDownSphereRadius);

            Gizmos.color = IsGrounded ? groundedSphereColor : ungroundedSphereColor;
            Gizmos.DrawWireSphere(origin + -transform.up * m_groundCheckDistance, m_castDownSphereRadius);


            if (IsObstacleBelow)
            {
                Gizmos.color = belowHitSphereColor;
                Gizmos.DrawWireSphere(m_belowHit.point, m_castDownSphereRadius);
            }
        }
    }
}
