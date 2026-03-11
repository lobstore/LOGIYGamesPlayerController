using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationsModule : MonoModuleBase
    {
        [SerializeField] Character character;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float rotationAnimationsBlendTime;


        private void Start()
        {
            character.OnJump.AddListener(() => { PlayAnimation("Jump"); });
            character.OnRoll.AddListener(() => { PlayAnimation("Roll"); });
            character.OnBackTurn.AddListener(() => PlayAnimation("BackTurn"));
        }
        public void PlayAnimation(string animname)
        {
            animator.CrossFade(animname, 0.05f);
        }

        public override void OnFixedUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            animator.SetFloat("Speed", character.SpeedMultiplier);
            if (character.CurrentRotationStrategy is CameraRelativeRotation or InputRelativeRotation)
            {

                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", character.SpeedMultiplier);
            }
            else
            {
                animator.SetFloat("VerticalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).z);
                animator.SetFloat("HorizontalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).x);

            }

            animator.SetBool("IsMoving", character.Input.MovementInput.magnitude > 0);
            animator.SetBool("IsGrounded", character.IsGrounded);
            animator.SetBool("IsFalling", character.IsFalling);
            animator.SetBool("IsSliding", character.IsSliding);

            animator.SetFloat("TurnAngle", character.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }

    }
}
