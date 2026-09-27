using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class CharacterVelocityDebug : MonoBehaviour
    {
        [SerializeField] Actor actor;
        [SerializeField] MovementWrapperBase controller;
        [SerializeField] Animator animator;
        [Header("Target Velocity")]
        [SerializeField] Color movementTargetVelocityArrowColor;
        [Header("Animator Velocity")]
        [SerializeField] Color animatorVelocityArrowColor;
        [Header("Actual Velocity")]
        [SerializeField] Color totalVelocityArrowColor;
        private void Update()
        {
            var velo = actor.RuntimeMovement.TargetDirection * actor.RuntimeMovement.CurrentSpeed * 3.5f;
            if (velo.magnitude > 0)
            {
                DebugDraw.DrawArrow(transform.position, velo, movementTargetVelocityArrowColor);
            }
            if (animator.velocity != Vector3.zero)
            {
                DebugDraw.DrawArrow(transform.position, animator.velocity, animatorVelocityArrowColor);

            }
            if (controller.Velocity != Vector3.zero)
            {
                DebugDraw.DrawArrow(transform.position, controller.Velocity, totalVelocityArrowColor);

            }
        }
    }
}
