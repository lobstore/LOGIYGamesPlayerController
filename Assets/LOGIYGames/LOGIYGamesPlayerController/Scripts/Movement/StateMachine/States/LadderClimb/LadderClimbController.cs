using LOGIYGames.Shared.Extensions;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class LadderClimbController : MonoBehaviour
    {
        Actor actor;
        MovementWrapperBase controller;
        private GameObject LDR;

        public bool IsInTrigger { get; private set; }

        [SerializeField] LayerMask include;
        Vector3 dir;
        [SerializeField] float baseSpeed = 3.5f;
        [SerializeField] float inertia;
        private void Awake()
        {
            actor = GetComponent<Actor>();
            controller = GetComponent<MovementWrapperBase>();

        }
        public void Enter()
        {
            controller.SetPosition(new Vector3(LDR.transform.position.x, controller.transform.position.y, LDR.transform.position.z));
            controller.UseGravity = false;
        }
        public void Climb()
        {
            Vector2 input = actor.Input.MovementInput;

            if (input.sqrMagnitude > 0.001f)
            {
                Vector3 targetDir = controller.Collider.transform.up * input.y;

                dir = Vector3.Lerp(
                    dir,
                    targetDir,
                    Time.deltaTime * inertia
                );
                //dir = Vector3.ProjectOnPlane(dir, actor.Sensors.LegsFrontHit.normal);
            }
            else
            {
                dir = controller.Velocity.normalized;
            }

            Vector3 velocity = dir * actor.RuntimeMovement.CurrentSpeed * baseSpeed;

            controller.ChangeHorizontalVelocity(velocity);
            controller.ChangeVerticalVelocity(velocity.y);
            Rotate();
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.IsLayerInMask(include))
            {
                IsInTrigger = true;
                LDR = other.gameObject;
            }
            else
            {
                IsInTrigger = false;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            IsInTrigger = false;
            LDR = null;
        }
        public void Exit()
        {
            controller.UseGravity = true;
        }
        public bool CanExit()
        {
            return actor.Input.InteractPressed ||
                     !IsInTrigger;
        }
        public bool CanEnter()
        {
            return IsInTrigger && actor.Input.InteractPressed;
        }
        void Rotate()
        {

            //controller.AddAcceleration(-actor.Sensors.LegsFrontHit.normal * 10);
            actor.RotateToDirection(LDR.transform.forward);
        }
    }
}
