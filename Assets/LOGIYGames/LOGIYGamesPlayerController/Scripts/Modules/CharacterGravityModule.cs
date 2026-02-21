using UnityEngine;
using LOGIYGames.CharacterCore;

namespace LOGIYGames
{
    /// <summary>
    /// Handles gravity for characters using either Unity CharacterController or KinematicCharacterController.
    /// Works with GenericControllerWrapper for seamless controller swapping.
    /// </summary>
    public class CharacterGravityModule : MonoModuleBase
    {
        [Header("Physics")]
        [SerializeField] bool useGravity;
        [SerializeField] private float groundMagnit;
        
        public Vector3 GravityDirection { get => gravityDirection.normalized; set => gravityDirection = value; }
        [SerializeField] Vector3 gravityDirection = new Vector3(0, -1, 0);
        
        public float BaseGravityForce = 9.84f;
        public float CurrentGravityForce;
        
        public Vector3 Velocity { get => velocity; set => velocity = value; }
        private Vector3 velocity;
        
        public bool UseGravity { get => useGravity; set => useGravity = value; }
        
        [Header("References")]
        private ControllerWrapperBase m_controllerWrapper;
        private SensorsModule m_sensors;
        private Character m_character;
        
        private void Awake()
        {
            m_controllerWrapper = GetComponent<ControllerWrapperBase>();
            m_sensors = GetComponent<SensorsModule>();
            m_character = GetComponent<Character>();
            
            Debug.Assert(m_controllerWrapper != null, "Error (CharacterGravityModule): Could not find GenericControllerWrapper component");
        }
        
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
        }
        
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            
            if (!useGravity) 
            { 
                CurrentGravityForce = 0; 
                return; 
            }
            
            // Check if grounded and on valid slope
            if (m_sensors != null && 
                m_sensors.IsGrounded &&
                Velocity.y < 0 &&
                m_sensors.IsValidSlope())
            {
                CurrentGravityForce = groundMagnit;
                Velocity = CurrentGravityForce * gravityDirection.normalized;
            }
            else
            {
                CurrentGravityForce = BaseGravityForce;
                Velocity += CurrentGravityForce * gravityDirection.normalized * Time.deltaTime;
            }
            
            // Check for overhead obstacles
            if (m_sensors != null && m_sensors.AboveHit.collider != null)
            {
                Velocity = GravityDirection.normalized * 0.5f;
            }
        }
    }
}
