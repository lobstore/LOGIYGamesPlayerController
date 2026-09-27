using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ClimbController : MonoBehaviour
    {
        [SerializeField] float baseSpeed = 3.5f;
        [SerializeField] float inertia;
        MovementWrapperBase controller;
        Actor actor;
        SensorsModule sensorModule;
        Vector3 dir;
        private void Awake()
        {
            sensorModule = GetComponent<SensorsModule>();
            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }
        public void Magnit()
        {
            controller.AddAcceleration(-sensorModule.LegsFrontHit.normal * 10);
        }
        public void Climb()
        {
            if (actor.Input.MovementInput.magnitude > 0)
            {

                dir = Vector3.Lerp(dir, actor.RuntimeMovement.TargetDirection, Time.deltaTime * inertia);
            }
            else
            {
                dir = controller.Velocity.normalized;
            }

            controller.ChangeVelocity(dir * actor.RuntimeMovement.CurrentSpeed * baseSpeed);
        }
    }
}
