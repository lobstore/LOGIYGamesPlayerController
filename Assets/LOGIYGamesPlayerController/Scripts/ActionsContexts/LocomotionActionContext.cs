using UnityEngine;
using LOGIYGames.CharacterCore;
namespace LOGIYGames
{
    [RequireComponent(typeof(Character))]
    public class LocomotionActionContext : GroundedActionContext
    {
        [SerializeField] float smoothTime = 0.3f;
        [SerializeField] float walkSpeed = 0.5f;
        [SerializeField] float runSpeed = 1f;
        [SerializeField] float sprintSpeed = 1.5f;
        [Header("Animation Parameters")]
        private int isMovingHash = Animator.StringToHash("IsMoving");
        private int yawInputHash = Animator.StringToHash("Yaw Input");
        private int speedHash = Animator.StringToHash("Speed");
        private int isSprintingHash = Animator.StringToHash("IsSprinting");
        private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        [SerializeField] AnimationCurve locomotionCurve;
        public bool IsTurning { get; set; }
        public bool IsSprinting { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();
        }
        private void FixedUpdate()
        {
            if (Character.SprintPressed)
            {
                IsSprinting = true;
                animator.SetBool(isSprintingHash, true);
                InternalSpeedMultiplier = sprintSpeed;
            }
            else
            {
                animator.SetBool(isSprintingHash, false);
                IsSprinting = false;
                InternalSpeedMultiplier = runSpeed;
            }
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();
            float animatedspeed = 0;
            animatedspeed = locomotionCurve.Evaluate(Character.TotalSpeedMultiplier);
            isMoving = MovementInput.magnitude > 0;
            animator.SetBool(isMovingHash, isMoving);
            animator.SetFloat(yawInputHash, Mathf.Clamp(deltaYaw, -1, 1), smoothTime, Time.deltaTime);
            animator.SetFloat(speedHash, animatedspeed);
            if (CameraManager.Instance.CameraFocusingState == CameraFocusingState.Focus)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(Character.HorizontalVelocity);
                localVelocity.Normalize();
                animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed);
                animator.SetFloat(verticalSpeedHash, localVelocity.z * animatedspeed);
            }
            else
            {

                animator.SetFloat(verticalSpeedHash, animatedspeed);
                animator.SetFloat(horizontalSpeedHash, 0);

            }
        }
        public override void EnterState()
        {
            base.EnterState();
            InternalSpeedMultiplier = runSpeed;
        }
        public override void ExitState()
        {
            base.ExitState();
            animator.SetFloat(speedHash, 0);
            animator.SetFloat(verticalSpeedHash, 0);
            animator.SetFloat(horizontalSpeedHash, 0);
            animator.SetBool(isSprintingHash, false);
        }

    }
}