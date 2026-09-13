using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationController : MonoModuleBase
    {
        [SerializeField] Actor character;
        [SerializeField] MovementWrapperBase controller;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float rotationAnimationsBlendTime;
        [SerializeField][Range(0, 0.5f)] private float crossFadeSpeed;

        [SerializeField] CharacterAnimationsData _data;
        public bool UseRootMotion { get => animator.applyRootMotion; set => animator.applyRootMotion = value; }

        public Vector3 ScaledTargetDirection { get; set; }

        [SerializeField] AnimatorOverrideController defaultOverride;

        private void Start()
        {
            animator.runtimeAnimatorController = defaultOverride;
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
                    //case Direction.Left:
                    //    if (evt.fallingSpeed > -7)
                    //    {
                    //        PlayAnimation(_data.Landing_Light_Left);

                    //    }
                    //    else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                    //    {
                    //        PlayAnimation(_data.Landing_Hard_Forward);
                    //    }
                    //    else if (evt.fallingSpeed < -10)
                    //    {
                    //        PlayAnimation(_data.Landing_Break);
                    //    }
                    //    break;
                    //case Direction.Right:
                    //    if (evt.fallingSpeed > -7)
                    //    {
                    //        PlayAnimation(_data.Landing_Light_Right);

                    //    }
                    //    else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                    //    {
                    //        PlayAnimation(_data.Landing_Hard_Forward);
                    //    }
                    //    else if (evt.fallingSpeed < -10)
                    //    {
                    //        PlayAnimation(_data.Landing_Break);
                    //    }
                    //    break;
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
                    //case Direction.Backward:
                    //    if (evt.fallingSpeed > -7)
                    //    {
                    //        PlayAnimation(_data.Landing_Light_Backward);

                    //    }
                    //    else if (evt.fallingSpeed < -7 && evt.fallingSpeed > -10)
                    //    {
                    //        PlayAnimation(_data.Landing_Hard_Forward);
                    //    }
                    //    else if (evt.fallingSpeed < -10)
                    //    {
                    //        PlayAnimation(_data.Landing_Break);
                    //    }
                    //    break;
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
                if (evt.movementSpeed > GetStateSpeed<WalkMovementState>())
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
                if (evt.movementSpeed > GetStateSpeed<WalkMovementState>())
                {
                    if (evt.angle > 0)
                    {
                        PlayAnimation(_data.Run_BackTurn_Right);

                    }
                    else
                    {
                        PlayAnimation(_data.Run_BackTurn_Left);

                    }
                }
                else if (evt.movementSpeed < GetStateSpeed<RunMovementState>())
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
            });
            character.EventBus.Subscribe<MovementStoppedEvent>((evt) =>
            {
                switch (evt.direction)
                {
                    case Direction.Forward:
                        if (evt.movementSpeed <= GetStateSpeed<WalkMovementState>())
                        {
                            PlayAnimation(_data.Walk_Stop_Forward);
                        }
                        else if (evt.movementSpeed <= GetStateSpeed<RunMovementState>())
                        {
                            PlayAnimation(_data.Run_Stop_Forward);
                        }
                        else if (evt.movementSpeed > GetStateSpeed<RunMovementState>())
                        {
                            PlayAnimation(_data.Sprint_Stop_Forward);
                        }
                        break;
                    case Direction.Backward:
                        if (evt.movementSpeed <= GetStateSpeed<WalkMovementState>())
                        {
                            PlayAnimation(_data.Walk_Stop_Backward);
                        }
                        else if (evt.movementSpeed <= GetStateSpeed<RunMovementState>())
                        {
                            PlayAnimation(_data.Run_Stop_Backward);
                        }
                        break;

                    default:
                        break;
                }

            });
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
            character.EventBus.Subscribe<WeaponEquipEvent>((evt) =>
            {
                switch (evt.WeaponEquipState)
                {
                    case WeaponEquipState.Equiped:
                        animator.runtimeAnimatorController = evt.WeaponData.AnimatorOverride;
                        if (evt.WeaponData.TwoHandsRequired)
                        {
                            PlayAnimation("Equip", 3);
                        }
                        else
                        {
                            switch (evt.WeaponSlotType)
                            {
                                case WeaponSlotType.RightHand:
                                    PlayAnimation("Equip", 1);
                                    break;
                                case WeaponSlotType.LeftHand:
                                    PlayAnimation("Equip", 2);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case WeaponEquipState.Unequiped:
                        animator.runtimeAnimatorController = defaultOverride;
                        if (evt.WeaponData.TwoHandsRequired)
                        {
                            PlayAnimation("Unequip", 3);

                        }
                        else
                        {
                            switch (evt.WeaponSlotType)
                            {
                                case WeaponSlotType.RightHand:
                                    PlayAnimation("Unequip", 1);
                                    break;
                                case WeaponSlotType.LeftHand:
                                    PlayAnimation("Unequip", 2);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

            });
            //character.EventBus.Subscribe<ComboAttackEvent>((evt) =>
            //{
            //    PlayAnimation(evt.AnimationData.AnimationName);
            //    animator.applyRootMotion = evt.AnimationData.UseRootMotion;
            //    animator.SetFloat("MotionSpeed", evt.AnimationData.MotionSpeed);
            //});
        }
        public void PlayAnimation(string animname, int layer = 0)
        {
            animator.CrossFade(animname, crossFadeSpeed, layer);
        }
        public void PlayAnimation(int animhash, int layer = 0)
        {
            animator.CrossFade(animhash, crossFadeSpeed, layer);
        }
        Vector2 input;
        public override void OnFixedUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);

            animator.SetFloat("Speed", character.RuntimeMovement.CurrentSpeed, crossFadeSpeed, Time.deltaTime);
            if (character.RotationStrategy is CharacterRelativeRotation or InputRelativeRotation)
            {

                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", character.RuntimeMovement.CurrentSpeed, crossFadeSpeed, Time.deltaTime);
            }
            else
            {
                if (character.Input.MovementInput.magnitude > 0)
                {
                    input = character.Input.MovementInput;
                }
                var snapped = SnapDirection(input);
                var horizontal = snapped.x;
                var vertical = snapped.y;
                animator.SetFloat("VerticalSpeed", vertical, crossFadeSpeed, Time.deltaTime);
                animator.SetFloat("HorizontalSpeed", horizontal, crossFadeSpeed, Time.deltaTime);

            }

            animator.SetBool("IsMoving", character.Input.MovementInput.magnitude > 0);
            animator.SetBool("IsGrounded", character.IsGrounded);
            animator.SetBool("IsFalling", character.GetMovementState<FallingMovementState>().IsActiveState);
            animator.SetBool("IsFocusing", character.Input.FocusPressed);
            animator.SetFloat("InputX", character.Input.MovementInput.x, crossFadeSpeed, Time.deltaTime);
            animator.SetFloat("InputY", character.Input.MovementInput.y, crossFadeSpeed, Time.deltaTime);

            animator.SetFloat("TurnAngle", character.RuntimeMovement.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }
        private Vector2 SnapDirection(Vector2 input)
        {

            var angle = Mathf.Atan2(input.x, input.y);
            var sector = Mathf.Round(angle / (Mathf.PI / 4f));

            var snappedAngle = sector * (Mathf.PI / 4f);

            return new Vector2(
                Mathf.Sin(snappedAngle),
                Mathf.Cos(snappedAngle)
            );
        }
        private void Update()
        {
            if (character.Input.MovementInput.magnitude > 0)
            {
                ScaledTargetDirection = Vector3.Lerp(ScaledTargetDirection, character.RuntimeMovement.TargetDirection.normalized, character.RuntimeMovement.Acceleration * Time.deltaTime);
            }
            else
            {
                ScaledTargetDirection = Vector3.Lerp(ScaledTargetDirection, Vector3.zero, character.RuntimeMovement.Deceleration * Time.deltaTime);
            }
        }
        private void OnAnimatorMove()
        {
            if (!animator.applyRootMotion)
                return;

            Vector3 delta = animator.velocity;

            controller.ForceMove(delta);
            transform.rotation *= animator.deltaRotation;
        }

        private float GetStateSpeed<T>() where T : MovementStateBase
        {
            return character.MovementStateMachine.GetState<T>().Data.TargetSpeed;
        }
    }
}
