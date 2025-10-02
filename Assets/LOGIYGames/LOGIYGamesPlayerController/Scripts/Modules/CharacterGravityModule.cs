using UnityEngine;
using LOGIYGames.CharacterCore;
namespace LOGIYGames
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterGravityModule : MonoModuleBase
    {
        [Header("Physics")]
        [SerializeField] bool useGravity;
        [SerializeField] float gravityMultiplier;
        [SerializeField] private float groundMagnit;
        public float VerticalVelocity { get => verticalVelocity; set => verticalVelocity = value; }
        private float verticalVelocity;
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

        private void ApplyGravity(float fixedDeltaTime)
        {
            if (!useGravity) { return; }
            if (Sensors.IsGrounded
                && VerticalVelocity < 0)
            {
                VerticalVelocity = -groundMagnit;
            }
            else
            {
                VerticalVelocity += Physics.gravity.y * fixedDeltaTime * gravityMultiplier;
            }
            controller.Move(new Vector3(0, VerticalVelocity, 0) * fixedDeltaTime);
        }
    }
}
