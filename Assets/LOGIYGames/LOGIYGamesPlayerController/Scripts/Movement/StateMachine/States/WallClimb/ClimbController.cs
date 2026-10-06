using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Extensions;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
        [SerializeField] LayerMask include;
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
        public bool CanEnter()
        {
           return actor.Sensors.IsObstacleLegsFront &&
                    actor.Sensors.LegsFrontHit.collider.gameObject.IsLayerInMask(include) &&
                    actor.Input.MovementInput.magnitude > 0
                        && (actor.transform.InverseTransformDirection(controller.Velocity).z > 0
                        || actor.transform.InverseTransformDirection(controller.Velocity).y > 0);
        }
        public bool CanExit()
        {
           return actor.Input.InteractPressed ||
                    !actor.Sensors.IsObstacleLegsFront ||
                    !actor.Sensors.LegsFrontHit.collider.gameObject.IsLayerInMask(include) ||
                    (actor.IsGrounded && actor.Input.MovementInput.y < 0);
        }
    }
}
