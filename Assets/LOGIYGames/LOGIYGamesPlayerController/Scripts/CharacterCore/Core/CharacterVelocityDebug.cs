using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class CharacterVelocityDebug : MonoBehaviour
    {
        [SerializeField] CharacterModule characterModule;
        [SerializeField] MovementWrapperBase controller;
        [SerializeField] Animator animator;
        [Header("Target Velocity")]
        [SerializeField] Color movementTargetDirectionArrowColor;
        [Header("Animator Velocity")]
        [SerializeField] Color animatorVelocityArrowColor;
        [Header("Actual Velocity")]
        [SerializeField] Color totalVelocityArrowColor;

        private void Update()
        {
            var velo = characterModule.TargetDirection * characterModule.BaseSpeed;
            if (velo.magnitude > 0)
            {
                DebugDraw.DrawArrow(transform.position, velo, movementTargetDirectionArrowColor);
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
