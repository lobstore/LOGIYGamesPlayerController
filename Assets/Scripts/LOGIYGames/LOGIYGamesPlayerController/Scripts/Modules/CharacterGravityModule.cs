using UnityEngine;
using LOGIYGames.CharacterCore;
namespace LOGIYGames
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterGravityModule : MonoModuleBase
    {
        [Header("Physics")]
        [SerializeField] bool useGravity;
        [SerializeField] private float groundMagnit;
        public Vector3 GravityDirection { get => gravityDirection.normalized; set => value = gravityDirection; }
        [SerializeField] Vector3 gravityDirection = new Vector3(0, -1,0);
        public float BaseGravityForce = 9.84f;
        public float CurrentGravityForce;
        public Vector3 Velocity { get => velocity; set => velocity = value; }
        private Vector3 velocity;
        public bool UseGravity { get => useGravity; set => useGravity = value; }
        [Header("References")]
        private CharacterController controller = null;
        private SensorsModule Sensors = null;
        private Character character = null;
        private void Awake()
        {

            controller = GetComponent<CharacterController>();
            Sensors = GetComponent<SensorsModule>();
            character = GetComponent<Character>();
        }
        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
            ApplyGravity(fixedDeltaTime);
        }
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (!useGravity) { CurrentGravityForce = 0; return; }
            if (Sensors.IsGrounded
                && Velocity.y < 0
                && Sensors.IsValidSlope())
            {
                CurrentGravityForce = groundMagnit;
                Velocity = CurrentGravityForce * gravityDirection.normalized;
            }
            else
            {
                CurrentGravityForce = BaseGravityForce;
                Velocity += CurrentGravityForce * gravityDirection.normalized * Time.deltaTime;
            }
            if (Sensors.AboveHit.collider!=null)
            {
                Velocity = GravityDirection.normalized * 0.5f;
            }
        }

        private void ApplyGravity(float fixedDeltaTime)
        {
            controller.Move(Velocity * fixedDeltaTime);
        }

    }
}
