using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using UnityEngine;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationModule : MonoModuleBase
    {
        [SerializeField] Character character;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float rotationAnimationsBlendTime;

        [SerializeField] CharacterAnimationsData _data;
        [SerializeField] private float crossFadeSpeed;

        private void Start()
        {
            character.EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        switch (evt.direction)
                        {
                            case Direction.Left:
                                PlayAnimation(_data.Jump_Grounded_Left);
                                break;
                            case Direction.Right:
                                PlayAnimation(_data.Jump_Grounded_Right);
                                break;
                            case Direction.Forward:
                                PlayAnimation(_data.Jump_Grounded_Forward);
                                break;
                            case Direction.Backward:
                                PlayAnimation(_data.Jump_Grounded_Backward);
                                break;
                            case Direction.Up:
                                PlayAnimation(_data.Jump_Grounded_Up);
                                break;
                            default:
                                break;
                        }
                        break;
                    case JumpType.HangJump:
                        PlayAnimation(_data.Jump_Braced_Backward);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    default:
                        break;
                }


                character.EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        switch (evt.direction)
                        {
                            case Direction.Left:
                                PlayAnimation(_data.Jump_Grounded_Left);
                                break;
                            case Direction.Right:
                                PlayAnimation(_data.Jump_Grounded_Right);
                                break;
                            case Direction.Forward:
                                PlayAnimation(_data.Jump_Grounded_Forward);
                                break;
                            case Direction.Backward:
                                PlayAnimation(_data.Jump_Grounded_Backward);
                                break;
                            case Direction.Up:
                                PlayAnimation(_data.Jump_Grounded_Up);
                                break;
                            default:
                                break;
                        }
                        break;
                    case JumpType.HangJump:
                        PlayAnimation(_data.Jump_Braced_Backward);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    default:
                        break;
                }


            });
            });
            character.EventBus.Subscribe<LandedEvent>((evt) =>
        {
            print(evt.horizontalDirection);
            print(evt.fallingSpeed);

            switch (evt.horizontalDirection)
            {
                case Direction.Left:
                    if (evt.fallingSpeed > -5)
                    {
                        PlayAnimation(_data.Landing_Light_Idle);

                    }
                    else if (evt.fallingSpeed < -5)
                    {
                        PlayAnimation(_data.Landing_Hard_Idle);
                    }
                    break;
                case Direction.Right:
                    if (evt.fallingSpeed > -5)
                    {
                        PlayAnimation(_data.Landing_Light_Idle);

                    }
                    else if (evt.fallingSpeed < -5)
                    {
                        PlayAnimation(_data.Landing_Hard_Idle);
                    }
                    break;
                case Direction.Forward:
                    if (evt.fallingSpeed > -5)
                    {
                        PlayAnimation(_data.Landing_Light_Idle);

                    }
                    else if (evt.fallingSpeed < -5)
                    {
                        PlayAnimation(_data.Landing_Hard_Idle);
                    }
                    break;
                default:
                    break;
            }

        });
            character.EventBus.Subscribe<RollPerformedEvent>((evt) => { PlayAnimation(_data.Roll_Forward); });
            character.EventBus.Subscribe<DashPerformedEvent>((evt) =>
            {

                switch (evt.direction)
                {
                    case Direction.Left:
                        PlayAnimation(_data.Dash_Left);
                        break;
                    case Direction.Right:
                        PlayAnimation(_data.Dash_Right);
                        break;
                    case Direction.Forward:
                        PlayAnimation(_data.Dash_Forward);
                        break;
                    case Direction.Backward:
                        PlayAnimation(_data.Dash_Backward);
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
                        PlayAnimation(_data.Run_BackTurn_Right);

                    }
                    else
                    {
                        PlayAnimation(_data.Run_BackTurn_Left);

                    }
                    PlayAnimation(_data.Sprint_BackTurn_Left);
                }
                else if (evt.movementSpeed < 0.5 && evt.movementSpeed > 0.2f)
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation(_data.Walk_BackTurn_Right);

                    }
                    else
                    {
                        PlayAnimation(_data.Walk_BackTurn_Left);

                    }
                }
                else if (evt.movementSpeed < 0.2)
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation(_data.Idle_BackTurn_Right);

                    }
                    else
                    {
                        PlayAnimation(_data.Idle_BackTurn_Left);

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
            character.EventBus.Subscribe<SlipPerformedEvent>((evt) =>
            {
                PlayAnimation("Slipjump");
            }
            );
            character.EventBus.Subscribe<MovementStoppedEvent>((evt) =>
            {
                switch (evt.direction)
                {
                    case Direction.Left:
                        if (evt.speed < 0.5)
                        {
                            PlayAnimation(_data.Walk_Stop_Left);
                        }
                        else if (evt.speed < 1)
                        {
                            PlayAnimation(_data.Run_Stop_Left);
                        }
                        break;
                    case Direction.Right:
                        if (evt.speed < 0.5)
                        {
                            PlayAnimation(_data.Walk_Stop_Right);
                        }
                        else if (evt.speed < 1)
                        {
                            PlayAnimation(_data.Run_Stop_Right);
                        }
                        break;
                    case Direction.Forward:
                        if (evt.speed < 0.5)
                        {
                            PlayAnimation(_data.Walk_Stop_Forward);
                        }
                        else if (evt.speed < 1)
                        {
                            PlayAnimation(_data.Run_Stop_Forward);
                        }
                        break;
                    case Direction.Backward:
                        if (evt.speed < 0.5)
                        {
                            PlayAnimation(_data.Walk_Stop_Backward);
                        }
                        else if (evt.speed < 1)
                        {
                            PlayAnimation(_data.Run_Stop_Backward);
                        }
                        break;

                    default:
                        break;
                }

            }

            );
        }
        public void PlayAnimation(string animname)
        {
            animator.CrossFade(animname, crossFadeSpeed);
        }
        public void PlayAnimation(int animhash)
        {
            animator.CrossFade(animhash, crossFadeSpeed);
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
            animator.SetBool("IsWallClimbing", character.IsWallClimbing);

            animator.SetFloat("TurnAngle", character.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }

    }
}
