using UnityEngine;

namespace LOGIYGames
{
    public class AnimatorControllerWrapper : ControllerWrapperBase
    {
        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private CharacterGravityModule m_characterGravityModule;
        [SerializeField] private SensorsModule m_sensors;

        [SerializeField]
        private bool m_applyGravityWhenGrounded = false;
        public override bool IsGrounded { get { return m_sensors.IsGrounded; } }

        public override bool ApplyGravityWhenGrounded { get { return m_applyGravityWhenGrounded; } }

        public override Vector3 Velocity { get { return m_characterController.velocity; } }

        private bool m_enableCollision = true;

        public override float MaxStepHeight
        {
            get { return m_characterController.stepOffset; }
            set { m_characterController.stepOffset = value; }
        }

        public override float Height
        {
            get { return m_characterController.height; }
            set { m_characterController.height = value; }
        }

        public override float SlopeLimit
        {
            get => m_characterController.slopeLimit;
            set => m_characterController.slopeLimit = Mathf.Max(0, value);
        }

        public override Vector3 Center
        {
            get { return m_characterController.center; }
            set { m_characterController.center = value; }
        }

        public override float Radius
        {
            get { return m_characterController.radius; }
            set { m_characterController.radius = value; }
        }


        public override bool CollisionEnabled
        {
            get { return m_enableCollision; }
            set
            {
                m_enableCollision = value;
            }
        }


        public override Vector3 GetCachedMoveDelta()
        {
            return Vector3.zero;
        }

        public override Quaternion GetCachedRotDelta()
        {
            return Quaternion.identity;
        }

        public override Collider GetCollider()
        {
            return m_characterController;
        }

        public override void Initialize()
        {
  
        }

        public override void Jump(float force)
        {
            if (m_characterGravityModule != null)
            {
                m_characterGravityModule.Velocity = transform.up * Mathf.Sqrt(force * -2f * Physics.gravity.y);
            }
        }

        public override void Move(Vector3 a_move)
        {
            
        }

        public override void Rotate(Quaternion a_targetRotation)
        {
            transform.rotation = a_targetRotation;
        }

        public override void SetPosition(Vector3 a_position)
        {
        }

        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            m_characterController.Move(m_characterGravityModule.Velocity*Time.deltaTime);
        }
    }
}
