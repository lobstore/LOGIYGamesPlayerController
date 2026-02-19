using UnityEngine;
using UnityEngine.Assertions;

namespace LOGIYGames
{

    [RequireComponent(typeof(CharacterController))]
    public class UnityControllerWrapper : GenericControllerWrapper
    {
        [SerializeField]
        private bool m_applyGravityWhenGrounded = false;

        private CharacterController m_characterController;
        private CharacterGravityModule m_characterGravityModule;

        private bool m_enableCollision = true;

        public override bool IsGrounded { get { return m_characterController.isGrounded; } }
        public override bool ApplyGravityWhenGrounded { get { return m_applyGravityWhenGrounded; } }
        public override Vector3 Velocity { get { return m_characterController.velocity; } }
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
        public override float SlopeLimit { get => m_characterController.slopeLimit; set => m_characterController.slopeLimit = Mathf.Max(0, value); }
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

        public override void Initialize()
        {
            m_characterController.enableOverlapRecovery = true;
        }

        public override Vector3 GetCachedMoveDelta() { return Vector3.zero; }
        public override Quaternion GetCachedRotDelta() { return Quaternion.identity; }

        //Gets or sets whether collision is enabled (GenericControllerWrapper override)
        public override bool CollisionEnabled
        {
            get { return m_enableCollision; }
            set
            {
                if (m_enableCollision != value)
                {
                    if (m_enableCollision)
                        m_characterController.enabled = false;
                    else
                        m_characterController.enabled = true;
                }

                m_enableCollision = value;
            }
        }

        private void Awake()
        {
            m_characterController = GetComponent<CharacterController>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();
            Assert.IsNotNull(m_characterController, "Error (UnityControllerWrapper): Could not find" +
                "CharacterController component");

            m_characterController.enableOverlapRecovery = true;
        }

        public override void Move(Vector3 a_move)
        {
            if (m_enableCollision)
            {
                m_characterController.Move(a_move * Time.deltaTime);
            }
            else
            {
                m_characterController.transform.Translate(a_move * Time.deltaTime, Space.World);
            }
        }

        public override void MoveAndRotate(Vector3 a_move, Quaternion a_rotDelta)
        {
            m_characterController.transform.rotation *= a_rotDelta;

            if (m_enableCollision)
            {
                m_characterController.Move(a_move*Time.deltaTime);
            }
            else
            {
                m_characterController.transform.Translate(a_move * Time.deltaTime, Space.World);
            }
        }

        public override void Rotate(Quaternion a_rotDelta)
        {
            m_characterController.transform.rotation *= a_rotDelta;
        }

        private void OnEnable()
        {
            m_characterController.enabled = true;
        }

        private void OnDisable()
        {
            m_characterController.enabled = true;
        }
        public override void SetPosition(Vector3 a_position)
        {
            transform.position = a_position;
        }

        public override void SetRotation(Quaternion a_rotation)
        {
            transform.rotation = a_rotation;
        }

        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
            transform.SetPositionAndRotation(a_position, a_rotation);
        }
        public override void Jump(float force)
        {
            m_characterGravityModule.Velocity = transform.up * Mathf.Sqrt(force * -2f * Physics.gravity.y);
        }
    }
}
