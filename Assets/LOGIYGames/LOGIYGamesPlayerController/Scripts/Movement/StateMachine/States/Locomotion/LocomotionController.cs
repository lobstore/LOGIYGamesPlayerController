using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LocomotionController : MonoBehaviour
    {
        [SerializeField] float baseSpeed = 3.5f;
        [SerializeField] float inertia;
        MovementWrapperBase controller;
        Actor actor;
        Vector3 dir;
        private void Awake()
        {

            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }
        public void Move()
        {
            if (controller is NavMeshControllerWrapper)
            {
                return;
            }
            if (actor.Input.MovementInput.magnitude > 0)
            {

                dir = Vector3.Lerp(dir, actor.RuntimeMovement.TargetDirection, Time.deltaTime * inertia);
            }

            controller.ChangeHorizontalVelocity(dir * actor.RuntimeMovement.CurrentSpeed * baseSpeed);
        }

    }
}
