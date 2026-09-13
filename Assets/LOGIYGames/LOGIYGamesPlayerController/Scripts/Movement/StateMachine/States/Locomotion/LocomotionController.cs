using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LocomotionController : MonoBehaviour
    {
        [SerializeField] float baseSpeed = 3.5f;
        MovementWrapperBase controller;
        Actor actor;
        private void Awake()
        {

            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }
        public void Move()
        {
            controller.ChangeVelocity(new Vector3( actor.RuntimeMovement.TargetDirection.x ,0, actor.RuntimeMovement.TargetDirection.z) * actor.RuntimeMovement.CurrentSpeed * baseSpeed + Vector3.up * controller.Velocity.y) ;
        }

    }
}
