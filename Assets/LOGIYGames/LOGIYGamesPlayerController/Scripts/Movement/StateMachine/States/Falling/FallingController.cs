using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class FallingController : MonoBehaviour
    {
        MovementWrapperBase controller;
        Actor actor;
        [SerializeField] float acceleration = 3.5f;
        private void Awake()
        {
            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }

        public void Glide()
        {
            if (controller is NavMeshControllerWrapper)
            {
                return;
            }
            if (actor.Input.MovementInput.magnitude > 0)
            {

                controller.AddAcceleration(actor.RuntimeMovement.TargetDirection * acceleration);
            }

        }
    }
}
