using LOGIYGames;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterModule))]
public class LocomotionActionContext : GroundedActionContext
{
    [Header("Animation Parameters")]
    private int isMovingHash = Animator.StringToHash("IsMoving");
    private int yawInputHash = Animator.StringToHash("Yaw Input");
    private int speedHash = Animator.StringToHash("Speed");
    private int isSprintingHash = Animator.StringToHash("IsSprinting");
    private int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");

    [SerializeField] float smoothTime = 0.3f;


    public bool IsSprinting { get; private set; } = false;


    protected override void UpdateAnimations()
    {
        IsFocusing = animator.GetBool("IsFocusing");
        isMoving = MovementInput.magnitude > 0;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetFloat(speedHash, player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
        if (IsFocusing)
        {
            animator.SetFloat(verticalSpeedHash, MovementInput.y * player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, MovementInput.x * player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
        }
        else
        {
            animator.SetFloat(verticalSpeedHash, player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, 0);
        }
        animator.SetFloat(yawInputHash, Mathf.Clamp(deltaY, -1, 1), smoothTime, Time.deltaTime);
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
