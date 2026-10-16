using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LocomotionController : MonoBehaviour
    {
        [SerializeField] float baseSpeed = 3.5f;
        [SerializeField] float acceleration;
        MovementWrapperBase controller;
        Actor actor;
        private void Awake()
        {
            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }
        public void Move()
        {
            var vel = Vector3.MoveTowards(controller.Velocity, actor.RuntimeMovement.TargetDirection * actor.RuntimeMovement.CurrentSpeed * baseSpeed, Time.deltaTime * acceleration);
            controller.ChangeHorizontalVelocity(vel);
        }

    }
}
