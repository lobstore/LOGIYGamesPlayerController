using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Extensions;
using RealStep;
using Unity.AppUI.Core;
using UnityEngine;

namespace LOGIYGames
{
    public class WallClimbController : MonoBehaviour
    {
        [SerializeField] float baseSpeed = 3.5f;
        [SerializeField] float inertia;
        MovementWrapperBase controller;
        Actor actor;
        Vector3 dir;
        [SerializeField] LayerMask include;
        private void Awake()
        {
            controller = GetComponent<MovementWrapperBase>();
            actor = GetComponent<Actor>();
        }
        public void Enter()
        {
            controller.UseGravity = false;
            GetComponent<Animator>().SetBool("IsWallClimbing", true);
            GetComponent<FootIK>().enabled = false;
            //controller.SetPosition(new Vector3(actor.Sensors.LegsFrontHit.point.x - controller.transform.forward.x*0.2f, controller.transform.position.y, actor.Sensors.LegsFrontHit.point.z - controller.transform.forward.z * 0.2f));
        }
        public void Exit()
        {
            controller.UseGravity = true;
            GetComponent<Animator>().SetBool("IsWallClimbing", false);
            GetComponent<FootIK>().enabled = true; ;
        }
        public void Magnit()
        {
            controller.ForceMove(controller.Velocity - actor.Sensors.LegsFrontHit.normal);
            actor.RotateToDirection(-actor.Sensors.LegsFrontHit.normal);
        }

        public void Climb()
        {
            Vector2 input = actor.Input.MovementInput;

            if (input.sqrMagnitude > 0.001f)
            {
                Vector3 targetDir =
                   controller.Collider.transform.right * input.x +
                    controller.Collider.transform.up * input.y;

                dir = Vector3.Lerp(
                    dir,
                    targetDir,
                    Time.deltaTime * inertia
                );
                dir= Vector3.ProjectOnPlane(dir, actor.Sensors.LegsFrontHit.normal);
            }
            else
            {
                dir = controller.Velocity.normalized;
            }

            Vector3 velocity = dir * actor.RuntimeMovement.CurrentSpeed * baseSpeed;

            controller.ChangeHorizontalVelocity(velocity);
            controller.ChangeVerticalVelocity(velocity.y);
        }

        public bool CanEnter()
        {
            return actor.Sensors.GetPlaneAbsAngle(actor.Sensors.LegsFrontHit.normal) > actor.Sensors.MaxStableSlopeAngle &&
                     actor.Sensors.LegsFrontHit.collider.gameObject.IsLayerInMask(include);
        }
        public bool CanExit()
        {
            return actor.Input.InteractPressed ||
                     !actor.Sensors.IsObstacleLegsFront || actor.Sensors.GetPlaneAbsAngle(actor.Sensors.LegsFrontHit.normal) <= actor.Sensors.MaxStableSlopeAngle ||
                     !actor.Sensors.LegsFrontHit.collider.gameObject.IsLayerInMask(include) ||
                     (actor.IsGrounded && actor.Input.MovementInput.y < 0);
        }
    }
}
