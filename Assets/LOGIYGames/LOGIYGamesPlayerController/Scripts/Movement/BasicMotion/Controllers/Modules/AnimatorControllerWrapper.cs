using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class AnimatorControllerWrapper : ControllerWrapperBase
    {
        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private CharacterGravityModule m_characterGravityModule;
        [SerializeField] private SensorsModule m_sensors;
        [SerializeField] private Character character;

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

        public override bool UseGravity { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override Vector3 Velocity => throw new System.NotImplementedException();

        public override void AddForce(Vector3 force)
        {
            if (m_characterGravityModule != null)
            {
                character.VelocityData.Gravity= force;
            }
        }

        public override void Move(Vector3 a_move)
        {
            
        }

        public override void ResetVelocity()
        {
            
        }

        public override void SetRotation(Quaternion a_targetRotation)
        {
            transform.rotation = a_targetRotation;
        }

        public override void SetPosition(Vector3 a_position)
        {
        }
        void Update()
        {
            m_characterController.Move(character.VelocityData.Gravity * Time.deltaTime);
        }
    }
}
