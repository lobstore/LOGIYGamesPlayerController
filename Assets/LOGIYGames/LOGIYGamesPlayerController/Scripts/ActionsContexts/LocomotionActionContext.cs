using LOGIYGames.CharacterCore;
using UnityEngine;
namespace LOGIYGames
{
    [RequireComponent(typeof(Character))]
    public class LocomotionActionContext : GroundedActionContext
    {
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float walkSpeed = 0.5f;
        [SerializeField] private float runSpeed = 1f;
        [SerializeField] private float sprintSpeed = 1.5f;
        [Header("Animation Parameters")]
        private int isMovingHash = Animator.StringToHash("IsMoving");
        private int yawInputHash = Animator.StringToHash("Yaw Input");
        private int speedHash = Animator.StringToHash("Speed");
        private int isSprintingHash = Animator.StringToHash("IsSprinting");
        private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        [SerializeField] protected AnimationCurve locomotionCurve;
        public bool IsTurning { get; set; }
        public bool IsSprinting { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Character.IsUnderPlayerControl)
                if (Character.SprintPressed && MovementInput.y > 0.5f)
                {
                    IsSprinting = true;
                    animator.SetBool(isSprintingHash, true);
                    InternalSpeedMultiplier = sprintSpeed;
                    if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
                    {
                        CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 90f, Time.deltaTime);
                    }
                }
                else
                {
                    animator.SetBool(isSprintingHash, false);
                    IsSprinting = false;
                    InternalSpeedMultiplier = runSpeed;
                    if (CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
                    {
                        CameraManager.Instance.CurentCameraController.FOV = Mathf.Lerp(CameraManager.Instance.CurentCameraController.FOV, 60f, Time.deltaTime);
                    }
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
            animator.SetFloat(speedHash, animatedspeed, smoothTime, Time.deltaTime);
            switch (CameraManager.Instance.CameraFocusingState)
            {
                case CameraFocusingState.FreeLook:
                    {
                        animator.SetFloat(verticalSpeedHash, animatedspeed);
                        animator.SetFloat(horizontalSpeedHash, 0);
                    }
                    break;
                case CameraFocusingState.LookForward:
                    {
                        Vector3 localVelocity = transform.InverseTransformDirection(Character.HorizontalVelocity);
                        localVelocity.Normalize();
                        animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed, smoothTime, Time.deltaTime);
                        animator.SetFloat(verticalSpeedHash, localVelocity.z * animatedspeed, smoothTime, Time.deltaTime);
                    }
                    break;
                case CameraFocusingState.Focus:
                    {
                        Vector3 localVelocity = transform.InverseTransformDirection(Character.HorizontalVelocity);
                        localVelocity.Normalize();
                        animator.SetFloat(horizontalSpeedHash, localVelocity.x * animatedspeed, smoothTime, Time.deltaTime);
                        animator.SetFloat(verticalSpeedHash, localVelocity.z * animatedspeed, smoothTime, Time.deltaTime);
                    }
                    break;
                default:
                    break;
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