using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using UnityEngine;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationModule : MonoModuleBase
    {
        [SerializeField] CharacterModule character;
        [SerializeField] ControllerWrapperBase controller;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float rotationAnimationsBlendTime;
        [SerializeField][Range(0, 0.5f)] private float crossFadeSpeed;

        [SerializeField] CharacterAnimationsData _data;
        public bool UseRootMotion { get => animator.applyRootMotion; set => animator.applyRootMotion = value;  }
        private void Start()
        {
            character.EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        switch (evt.direction)
                        {
                            case Direction.Forward:
                                PlayAnimation(_data.Jump_Grounded_Forward);
                                break;
                            case Direction.Backward:
                                PlayAnimation(_data.Jump_Grounded_Backward);
                                break;
                            default:
                                PlayAnimation(_data.Jump_Grounded_Up);
                                break;
                        }
                        break;
                    case JumpType.HangJump:
                        PlayAnimation(_data.Jump_Braced_Backward);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    case JumpType.Dash:
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
                        break;
                    case JumpType.Slip:
                        PlayAnimation("Slipjump");
                        break;
                    case JumpType.Roll:
                        PlayAnimation(_data.Roll_Forward);
                        break;
                    default:
                        break;
                }
            });
            character.EventBus.Subscribe<LandedEvent>((evt) =>
            {
                switch (evt.horizontalDirection)
                {
                    case Direction.Left:
                        if (evt.fallingSpeed > -7)
                        {
                            PlayAnimation(_data.Landing_Light_Left);

                        }
                        else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                        {
                            PlayAnimation(_data.Landing_Hard_Forward);
                        }
                        else if (evt.fallingSpeed < -10)
                        {
                            PlayAnimation(_data.Landing_Break);
                        }
                        break;
                    case Direction.Right:
                        if (evt.fallingSpeed > -7)
                        {
                            PlayAnimation(_data.Landing_Light_Right);

                        }
                        else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                        {
                            PlayAnimation(_data.Landing_Hard_Forward);
                        }
                        else if (evt.fallingSpeed < -10)
                        {
                            PlayAnimation(_data.Landing_Break);
                        }
                        break;
                    case Direction.Forward:
                        if (evt.fallingSpeed > -7)
                        {
                            PlayAnimation(_data.Landing_Light_Forward);

                        }
                        else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                        {
                            PlayAnimation(_data.Landing_Hard_Forward);
                        }
                        else if (evt.fallingSpeed < -10)
                        {
                            PlayAnimation(_data.Landing_Break);
                        }
                        break;
                    case Direction.Backward:
                        if (evt.fallingSpeed > -7)
                        {
                            PlayAnimation(_data.Landing_Light_Backward);

                        }
                        else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                        {
                            PlayAnimation(_data.Landing_Hard_Forward);
                        }
                        else if (evt.fallingSpeed < -10)
                        {
                            PlayAnimation(_data.Landing_Break);
                        }
                        break;
                    case Direction.NoMovement:
                        if (evt.fallingSpeed > -7)
                        {
                            PlayAnimation(_data.Landing_Light_Idle);

                        }
                        else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                        {
                            PlayAnimation(_data.Landing_Break);
                        }
                        else if (evt.fallingSpeed < -10)
                        {
                            PlayAnimation(_data.Landing_Break);
                        }
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
                        PlayAnimation(_data.Run_Turn_90R);

                    }
                    else
                    {
                        PlayAnimation(_data.Run_Turn_90L);

                    }
                }
                else
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation(_data.Idle_Turn_90R);

                    }
                    else
                    {
                        PlayAnimation(_data.Idle_Turn_90L);

                    }
                }
            });
            character.EventBus.Subscribe<BackTurnPerformedEvent>((evt) =>
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
                        PlayAnimation(_data.Walk_BackTurn_Left);

                    }
                    else
                    {
                        PlayAnimation(_data.Walk_BackTurn_Left);

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
            character.EventBus.Subscribe<MovementStoppedEvent>((evt) =>
            {
                switch (evt.direction)
                {
                    //case Direction.Left:
                    //    if (evt.speed < 0.5)
                    //    {
                    //        PlayAnimation(_data.Walk_Stop_Left);
                    //    }
                    //    else if (evt.speed < 1)
                    //    {
                    //        PlayAnimation(_data.Run_Stop_Left);
                    //    }
                    //    break;
                    //case Direction.Right:
                    //    if (evt.speed < 0.5)
                    //    {
                    //        PlayAnimation(_data.Walk_Stop_Right);
                    //    }
                    //    else if (evt.speed < 1)
                    //    {
                    //        PlayAnimation(_data.Run_Stop_Right);
                    //    }
                    //    break;
                    case Direction.Forward:
                        if (evt.speed <= 0.5)
                        {
                            PlayAnimation(_data.Walk_Stop_Forward);
                        }
                        else if (evt.speed <= 1)
                        {
                            PlayAnimation(_data.Run_Stop_Forward);
                        }
                        else if (evt.speed > 1)
                        {
                            PlayAnimation(_data.Sprint_Stop_Forward);
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
            character.EventBus.Subscribe<MantlingEvent>((evt) =>
            {
                switch (evt.Type)
                {
                    case MantlingType.StepOnLow:
                        PlayAnimation("StepOn_Little");
                        break;
                    case MantlingType.StepOnHigh:
                        PlayAnimation("StepOn_High");
                        break;
                    case MantlingType.BracedLow:
                        PlayAnimation("Mantling_Low");
                        break;
                    case MantlingType.BracedHigh:
                        PlayAnimation("Mantling_High");
                        break;
                    default:
                        break;
                }
            });
            character.EventBus.Subscribe<WallrunEnterEvent>((evt) =>
            {
                if (evt.IsRightSide)
                {
                    PlayAnimation("Wallrun_RightSide");
                }
                else
                {
                    PlayAnimation("Wallrun_LeftSide");
                }
            });
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
            if (animator.velocity != Vector3.zero)
            {
                DebugDraw.DrawArrow(transform.position, animator.velocity, Color.white);

            }
            animator.SetFloat("Speed", character.SpeedMultiplier);
            if (character.RotationStrategy is ThirdPersonPlanarRotation or InputRelativeRotation)
            {

                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", character.Input.MovementInput.magnitude, 0.05f, Time.deltaTime);
            }
            else if (character.RotationStrategy is WallClimbRotaion)
            {
                animator.SetFloat("HorizontalSpeed", character.Input.MovementInput.x, 0.05f, Time.deltaTime);
                animator.SetFloat("VerticalSpeed", character.Input.MovementInput.y, 0.05f, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("VerticalSpeed", transform.InverseTransformDirection(character.ScaledTargetDirection).z);
                animator.SetFloat("HorizontalSpeed", transform.InverseTransformDirection(character.ScaledTargetDirection).x);
            }

            animator.SetBool("IsMoving", character.Input.MovementInput.magnitude > 0);
            animator.SetBool("IsGrounded", character.IsGrounded);
            animator.SetBool("IsFalling", character.IsFalling);
            animator.SetBool("IsSliding", character.IsSliding);
            animator.SetBool("IsFocusing", character.Input.FocusPressed);
            animator.SetBool("IsOnLadder", character.IsOnLadder);
            animator.SetBool("IsWallClimbing", character.IsWallClimbing);
            animator.SetBool("IsSwimming", character.IsSwimming);
            animator.SetBool("IsFlying", character.IsFlying);
            animator.SetBool("IsWallRunning", character.IsWallRunning);

            animator.SetFloat("TurnAngle", character.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }

        private void OnAnimatorMove()
        {
            if (animator.applyRootMotion)
            {
                character.VelocityData.Locomotion = new Vector3(animator.velocity.x, animator.velocity.y, animator.velocity.z);
                character.Move(character.VelocityData.Total);
                character.Rotate(animator.rootRotation);
            }
        }
    }
}
