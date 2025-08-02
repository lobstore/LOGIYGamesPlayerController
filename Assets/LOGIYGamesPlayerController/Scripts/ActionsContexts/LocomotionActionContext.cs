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
    private int runTurn180TriggerHash = Animator.StringToHash("BackTurn");
    [SerializeField] float smoothTime = 0.3f;

    public bool IsTurning { get; set; }
    public bool IsSprinting { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
        Input.SprintEvent.AddListener(Sprint);
    }

    private void Sprint(InputAction.CallbackContext arg0)
    {
        switch (arg0.phase)
        {
            case InputActionPhase.Performed:
                IsSprinting = true;
                animator.SetBool(isSprintingHash, true);
                InternalSpeedMultiplier = 1.5f;
                break;
            case InputActionPhase.Canceled:
                animator.SetBool(isSprintingHash, false);
                IsSprinting = false;
                InternalSpeedMultiplier = 1f;
                break;
            default:
                break;
        }
    }

    protected override void UpdateAnimations()
    {
        IsFocusing = animator.GetBool("IsFocusing");
        isMoving = MovementInput.magnitude > 0;
        animator.SetBool(isMovingHash, isMoving);
        animator.SetFloat(speedHash, player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
        if (IsFocusing)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(player.HorizontalVelocity);

            animator.SetFloat(horizontalSpeedHash, Mathf.Clamp(localVelocity.x, -1, 1) * player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            animator.SetFloat(verticalSpeedHash, Mathf.Clamp(localVelocity.z, -1, 1) * player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
        }
        else
        {

            animator.SetFloat(verticalSpeedHash, player.TotalSpeedMultiplier, smoothTime, Time.deltaTime);
            animator.SetFloat(horizontalSpeedHash, 0);
        }
        animator.SetFloat(yawInputHash, Mathf.Clamp(deltaY, -1, 1), smoothTime, Time.deltaTime);
    }
    protected override void Rotate()
    {
        if (!IsFocusing && Vector3.Angle(transform.forward, moveDirection) > 140 && !IsTurning)
        {
            Debug.Log(IsTurning);
            IsTurning = true;
            animator.SetTrigger(runTurn180TriggerHash);
            return;
        }
        else
        {
            base.Rotate();

        }
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
