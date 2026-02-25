using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationsModule : MonoModuleBase
    {
        [SerializeField] Character character;
        [SerializeField] SensorsModule sensors;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float animationsSmoothTime;

        private void Awake()
        {
            character.OnJumpPerformed.AddListener(() => { animator.CrossFade("Jump", 0.05f); });
            character.OnRollPerformed.AddListener(() => { animator.CrossFade("Roll", 0.05f); });
        }
        public override void OnFixedUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            animator.SetFloat("Speed", character.SpeedMultiplier, animationsSmoothTime, Time.deltaTime);
            if (character.CurrentRotationStrategy is CameraRelativeRotation or InputRelativeRotation)
            {
                animator.SetFloat("HorizontalSpeed", 0);
            }
            else
            {
                animator.SetFloat("HorizontalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).x, animationsSmoothTime, Time.deltaTime);

            }
            animator.SetFloat("VerticalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).z, animationsSmoothTime, Time.deltaTime);
            animator.SetBool("IsMoving", character.Velocity.magnitude > 0 || character.DeltaYaw != 0);
            animator.SetBool("IsFalling", !sensors.IsGrounded);
            animator.SetBool("IsGrounded", sensors.IsGrounded);
            animator.SetFloat("TurnAngle", character.DeltaYaw, animationsSmoothTime, Time.deltaTime);
        }

    }
}
