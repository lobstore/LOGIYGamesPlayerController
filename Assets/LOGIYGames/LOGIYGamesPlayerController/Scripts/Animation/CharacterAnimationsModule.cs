using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.Animation
{
    public class CharacterAnimationsModule : MonoModuleBase
    {
        [SerializeField] Character character;
        [SerializeField] SensorsModule sensors;
        [SerializeField] Animator animator;

        [SerializeField][Range(0, 0.5f)] private float locomotioAnimationsBlendTime;
        [SerializeField][Range(0, 0.5f)] private float rotationAnimationsBlendTime;


        private void Start()
        {
            character.OnJumpStart.AddListener(() => { PlayAnimation("Jump"); animator.SetBool("IsJumping",true); });
            character.OnJumpEnd.AddListener(() => { animator.SetBool("IsJumping",false); });
            character.OnRollStart.AddListener(() => { PlayAnimation("Roll"); animator.SetBool("IsRolling", true); });
            character.OnRollEnd.AddListener(() => { animator.SetBool("IsRolling", false); });
            character.OnBackTurnStart.AddListener(() => PlayAnimation("BackTurn"));
        }
        public void PlayAnimation(string animname)
        {
            animator.CrossFade(animname, 0.05f);
        }

        public override void OnFixedUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            animator.SetFloat("Speed", character.SpeedMultiplier, locomotioAnimationsBlendTime, Time.deltaTime);
            if (character.CurrentRotationStrategy is CameraRelativeRotation or InputRelativeRotation)
            {

                animator.SetFloat("HorizontalSpeed", 0);
                animator.SetFloat("VerticalSpeed", character.SpeedMultiplier, locomotioAnimationsBlendTime, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("VerticalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).z, locomotioAnimationsBlendTime, Time.deltaTime);
                animator.SetFloat("HorizontalSpeed", transform.InverseTransformDirection(character.Velocity.normalized).x, locomotioAnimationsBlendTime, Time.deltaTime);

            }

            animator.SetBool("IsMoving", character.Input.MovementInput.magnitude > 0);
            animator.SetBool("IsGrounded", sensors.IsGrounded);
            animator.SetFloat("TurnAngle", character.DeltaYaw, rotationAnimationsBlendTime, Time.deltaTime);
        }

    }
}
