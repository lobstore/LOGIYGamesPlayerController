using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class FallingController : MonoBehaviour
    {
        private MovementWrapperBase controller;
        private Actor actor;

        [SerializeField] private float acceleration = 3.5f;
        [SerializeField] private float maxHorizontalSpeed = 8f;

        private void Awake()
        {
            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }

        public void Glide()
        {
            Vector2 input = actor.Input.MovementInput;

            if (input.sqrMagnitude < 0.01f)
                return;

            Vector3 direction = actor.RuntimeMovement.TargetDirection.normalized;

            controller.AddAcceleration(direction * acceleration);

            Vector3 velocity = controller.Velocity; // Подставь свой getter скорости
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

            if (horizontalVelocity.magnitude > maxHorizontalSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;

                velocity.x = horizontalVelocity.x;
                velocity.z = horizontalVelocity.z;

                controller.ChangeHorizontalVelocity(velocity); // Подставь свой метод установки скорости
            }
        }
    }
}
