using LOGIYGames.CharacterCore;
using UnityEditor.UIElements;
using UnityEngine;

namespace LOGIYGames
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerWrapper : ControllerWrapperBase
    {
        [Header("Unity Controller Settings")]

        private CharacterController m_characterController;
        private CharacterGravityModule m_characterGravityModule;
        private Character m_character;
        private SensorsModule m_sensors;

        private Vector3 totalVelocity;
        Vector3 planarVelocity;
        Vector3 verticalVelocity;
        [SerializeField] private float projectingPlanarSpeed;
        [SerializeField] private float slopeSlideMaxSpeed;
        [SerializeField] private float slopeSlideAcceleration;

        #region Public Properties

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
        public override bool UseGravity { get => m_characterGravityModule.UseGravity; set => m_characterGravityModule.UseGravity = value; }

        public override Vector3 Velocity => m_characterController.velocity;
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            m_sensors = GetComponent<SensorsModule>();
            m_character = GetComponent<Character>();
            m_characterController = GetComponent<CharacterController>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();
            if (m_characterController == null)
            {
                m_characterController = gameObject.AddComponent<CharacterController>();
            }
            if (m_characterController == null)
            {
                m_characterGravityModule = GetComponent<CharacterGravityModule>();
            }

            m_characterController.enableOverlapRecovery = true;
        }

        private void LateUpdate()
        {
            DebugDraw.DrawArrow(transform.position, totalVelocity, Color.green);
        }
        private void Update()
        {
            if (!m_sensors.IsValidSlope() && verticalVelocity.y<0)
            {
                totalVelocity = Vector3.Lerp(totalVelocity, Vector3.ProjectOnPlane(Vector3.down* slopeSlideMaxSpeed, m_sensors.BelowHit.normal), Time.deltaTime * slopeSlideAcceleration);

            }
            else
            {
                totalVelocity = planarVelocity + verticalVelocity;
            }
        }
        #endregion

        #region Movement Methods

        public override void Move(Vector3 a_move)
        {

            planarVelocity = a_move;
            verticalVelocity = m_characterGravityModule.Velocity;
            if (m_sensors.IsOnSlope && UseProjectionOnPlane)
            {
                ProjectVelocity();
            }

            m_characterController.Move(totalVelocity * Time.deltaTime);
        }
        public override void ForceMove(Vector3 a_move)
        {
            m_characterController.Move(a_move * Time.deltaTime);
        }
        private void ProjectVelocity()
        {
            Vector3 projectedPlanarVelocity = Vector3.zero;
            projectedPlanarVelocity = Vector3.ProjectOnPlane(planarVelocity, m_sensors.BelowHit.normal);
            planarVelocity = Vector3.Lerp(planarVelocity, projectedPlanarVelocity, Time.deltaTime * projectingPlanarSpeed);
        }

        public override void SetRotation(Quaternion a_targetRotation)
        {
            m_characterController.transform.rotation = a_targetRotation;
        }

        #endregion

        #region Transform Methods

        public override void SetPosition(Vector3 a_position)
        {
            transform.position = a_position;
        }

        #endregion


        #region Jump Method

        public override void Jump(Vector3 force)
        {
            if (m_characterGravityModule != null)
            {
                m_characterGravityModule.Velocity = force;
            }
        }

        #endregion


        public override void ResetVelocity()
        {
            totalVelocity = Vector3.zero;
        }



    }
}
