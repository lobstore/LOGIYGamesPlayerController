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
            if (actor.Input.MovementInput.magnitude > 0)
            {

                dir = Vector3.Lerp(dir, new Vector3(actor.RuntimeMovement.TargetDirection.x, 0, actor.RuntimeMovement.TargetDirection.z), Time.deltaTime * inertia);
            }
            else
            {
                dir = controller.Velocity.normalized;
            }

            controller.ChangeVelocity(dir * actor.RuntimeMovement.CurrentSpeed * baseSpeed + Vector3.up * controller.Velocity.y);
        }

    }
}
