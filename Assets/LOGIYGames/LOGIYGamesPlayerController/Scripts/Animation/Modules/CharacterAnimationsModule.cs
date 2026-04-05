using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationsModule : MonoModuleBase
    {
        [SerializeField] Character character;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float rotationAnimationsBlendTime;

        public string _dashLeft;
        public string _dashRight;
        public string _dashForward;
        public string _dashBackward;

        private void Start()
        {

            character.EventBus.Subscribe<JumpPerformedEvent>((evt) => { PlayAnimation("Jump"); });
            character.EventBus.Subscribe<RollPerformedEvent>((evt) => { PlayAnimation("Roll"); });
            character.EventBus.Subscribe<DashPerformedEvent>((evt) => {

                switch (evt.direction)
                {
                    case Direction.Left:
                        PlayAnimation(_dashLeft);
                        break;
                    case Direction.Right:
                        PlayAnimation(_dashRight);
                        break;
                    case Direction.Forward:
                        PlayAnimation(_dashForward);
                        break;
                    case Direction.Backward:
                        PlayAnimation(_dashBackward);
                        break;
                    default:
                        break;
                }

            });
            character.EventBus.Subscribe<TurnPerformedEvent>((evt) =>
            {
                if (evt.movementSpeed > 0.5)
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation("BackTurnRunRight");

                    }
                    else
                    {
                        PlayAnimation("BackTurnRunLeft");

                    }
                    PlayAnimation("BackTurn");
                }
                else if (evt.movementSpeed < 0.5 && evt.movementSpeed > 0.2f)
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation("BackTurnWalkRight");

                    }
                    else
                    {
                        PlayAnimation("BackTurnWalkLeft");

                    }
                }
                else if (evt.movementSpeed < 0.2)
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation("BackTurnIdleRight");

                    }
                    else
                    {
                        PlayAnimation("BackTurnIdleLeft");

                    }
                }
            });
            character.EventBus.Subscribe<LeashWeaponEvent>((evt) =>
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
            if (character.RotationStrategy is CameraRelativeRotation or InputRelativeRotation)
            {

                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", character.Input.MovementInput.magnitude, 0.05f, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("VerticalSpeed", character.Input.MovementInput.y, 0.1f, Time.deltaTime);
                animator.SetFloat("HorizontalSpeed", character.Input.MovementInput.x, 0.1f, Time.deltaTime);
            }

            animator.SetBool("IsMoving", character.Input.MovementInput.magnitude > 0);
            animator.SetBool("IsGrounded", character.IsGrounded);
            animator.SetBool("IsFalling", character.IsFalling);
            animator.SetBool("IsSliding", character.IsSliding);
            animator.SetBool("IsFocusing", character.Input.FocusPressed);
            animator.SetBool("IsOnLadder", character.IsOnLadder);

            animator.SetFloat("TurnAngle", character.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }

    }
}
