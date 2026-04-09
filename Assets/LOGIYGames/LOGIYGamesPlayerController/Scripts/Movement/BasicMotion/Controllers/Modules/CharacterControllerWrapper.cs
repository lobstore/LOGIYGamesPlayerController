using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Wrapper for Unity's built-in CharacterController.
    /// Implements the GenericControllerWrapper interface to allow seamless swapping
    /// with KinematicControllerWrapper.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerWrapper : ControllerWrapperBase
    {
        [Header("Unity Controller Settings")]

        private CharacterController m_characterController;
        private CharacterGravityModule m_characterGravityModule;
        private Character m_character;
        private SensorsModule m_sensors;

        private Vector3 totalVelocity;
        private Vector3 totalVerticalVelocity;
        private Vector3 totalPlanarVelocity;
        Vector3 planarVelocity;
        Vector3 verticalVelocity;
        [SerializeField] private float projectingPlanarSpeed;
        [SerializeField] private float projectingVerticalSpeed;

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
            if (m_characterController == null)
            {
                m_characterController = gameObject.AddComponent<CharacterController>();
            }
            m_characterGravityModule = GetComponent<CharacterGravityModule>();

            m_characterController.enableOverlapRecovery = true;
        }

        private void LateUpdate()
        {
            DebugDraw.DrawArrow(transform.position, totalVelocity, Color.green);
        }

        #endregion

        #region Movement Methods

        public override void Move(Vector3 a_move)
        {
            planarVelocity = a_move;
            verticalVelocity = m_characterGravityModule.Velocity;


            if (m_sensors.IsValidSlope())
            {
                if (m_sensors.IsGrounded && m_characterGravityModule.Velocity.y < 0 && m_character.Input.MovementInput.magnitude > 0)
                {
                    ProjectVelocity();
                }
                else
                {
                    totalPlanarVelocity = planarVelocity;
                    totalVerticalVelocity = verticalVelocity;
                }

            }
            else
            {
                ProjectVelocity();
            }



            totalVelocity = totalVerticalVelocity + totalPlanarVelocity;

            m_characterController.Move(totalVelocity * Time.deltaTime);

        }
        public override void ForceMove( Vector3 a_move)
        {
            m_characterController.Move(a_move * Time.deltaTime);
        }
        private void ProjectVelocity()
        {
            Vector3 projectedPlanarVelocity = Vector3.zero;
            Vector3 projectedVerticalVelocity = Vector3.zero;
            projectedPlanarVelocity = Vector3.ProjectOnPlane(planarVelocity, m_sensors.BelowHit.normal);
            projectedVerticalVelocity = Vector3.ProjectOnPlane(verticalVelocity, m_sensors.BelowHit.normal);
            totalPlanarVelocity = Vector3.Lerp(totalPlanarVelocity, projectedPlanarVelocity, Time.deltaTime * projectingPlanarSpeed);
            totalVerticalVelocity = Vector3.Lerp(totalVerticalVelocity, projectedVerticalVelocity, Time.deltaTime * projectingVerticalSpeed);
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
