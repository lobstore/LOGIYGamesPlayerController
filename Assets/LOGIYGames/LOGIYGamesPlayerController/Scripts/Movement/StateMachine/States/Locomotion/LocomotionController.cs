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
            dir = Vector3.MoveTowards(controller.Velocity, actor.RuntimeMovement.TargetDirection * actor.RuntimeMovement.CurrentSpeed * baseSpeed, Time.deltaTime * inertia);

            controller.ChangeHorizontalVelocity(dir);
        }

    }
}
