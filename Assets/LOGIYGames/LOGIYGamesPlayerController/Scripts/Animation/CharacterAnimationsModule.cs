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
            character.EventBus.Subscribe<JumpPerformedEvent>((evt) => { PlayAnimation("Jump"); });
            character.EventBus.Subscribe<RollPerformedEvent>((evt) => { PlayAnimation("Roll"); });
            character.EventBus.Subscribe<TurnPerformedEvent>((evt) => PlayAnimation("BackTurn"));
            character.EventBus.Subscribe<OnLeashWeaponEvent>((evt) =>
            {
                if (evt.unleashWeapon)
                {
                    PlayAnimation("Unleash");
                    animator.SetBool("IsInCombat", true);
                }
                else
                {
                    PlayAnimation("Leash");
                    animator.SetBool("IsInCombat", false);
                }
            }
            );
        }
        public void PlayAnimation(string animname)
        {
            animator.CrossFade(animname, 0.05f);
        }

        public override void OnFixedUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            animator.SetFloat("Speed", character.SpeedMultiplier);
            if (character.CurrentRotationStrategy is CameraRelativeRotation or InputRelativeRotation or NoneRotation)
            {

                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", character.SpeedMultiplier);
            }
            else
            {
                //animator.SetFloat("VerticalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).z);
                //animator.SetFloat("HorizontalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).x);
                animator.SetFloat("VerticalSpeed", character.Input.MovementInput.y, 0.05f, Time.deltaTime);
                animator.SetFloat("HorizontalSpeed", character.Input.MovementInput.x, 0.05f, Time.deltaTime);
            }

            animator.SetBool("IsMoving", character.Input.MovementInput.magnitude > 0);
            animator.SetBool("IsGrounded", character.IsGrounded);
            animator.SetBool("IsFalling", character.IsFalling);
            animator.SetBool("IsSliding", character.IsSliding);
            animator.SetBool("IsFocusing", character.Input.FocusPressed);

            animator.SetFloat("TurnAngle", character.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }

    }
}
