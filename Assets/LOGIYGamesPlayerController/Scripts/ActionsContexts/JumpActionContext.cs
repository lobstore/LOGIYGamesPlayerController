using LOGIYGames;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpActionContext : AerialActionContext
{

    [Header("Jump Settings")]
    [SerializeField] private float jumpVerticalImpulse = 1.5f;
    [SerializeField] private float jumpPlanarImpulse = 1f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private int maxJumpCount = 2;
    [field: SerializeField] public bool CanJump { get; set; } = true;

    // State Variables
    private CountdownTimer jumpCooldownTimer;
    private int currentJumpCount;
    public bool IsJumping { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeJumpSystem();
    }

    private void InitializeJumpSystem()
    {
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        player.PlayerTimers.Add(jumpCooldownTimer);
        currentJumpCount = maxJumpCount;
    }

    private void OnEnable()
    {
        jumpCooldownTimer.Reset(jumpCooldown);
        Input.JumpEvent.AddListener(PerformJump);
        jumpCooldownTimer.OnTimerStart += StartJump;
        jumpCooldownTimer.OnTimerStop += StopJump;

    }



    private void OnDisable()
    {
        Input.JumpEvent.RemoveListener(PerformJump);
        jumpCooldownTimer.OnTimerStart -= StartJump;
        jumpCooldownTimer.OnTimerStop -= StopJump;

    }
    private void PerformJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (CanJump && currentJumpCount > 0 && !jumpCooldownTimer.IsRunning)
                {
                    jumpCooldownTimer.Start();
                }
                break;
            default:
                break;
        }
    }
    private void StartJump()
    {
        IsJumping = true;

    }
    private void StopJump()
    {
        IsJumping = false;
    }

    private void ExecuteJump()
    {
        switch (MotionType)
        {
            case MotionType.RigidBody:
                {
                    var rb = GetComponent<Rigidbody>();
                    rb.AddForce(Vector3.up * jumpVerticalImpulse * -2f * Physics.gravity.y, ForceMode.Impulse);
                    if (MovementInput.magnitude > 0)
                    {

                        rb.AddForce((player.transform.forward * MovementInput.y + player.transform.right * player.TotalSpeedMultiplier * MovementInput.x) * jumpPlanarImpulse, ForceMode.Impulse);
                    }
                }
                break;
            case MotionType.CharacterController:
                {
                    player.VerticalVelocity = Mathf.Sqrt(jumpVerticalImpulse * -2f * Physics.gravity.y);
                    if (MovementInput.magnitude > 0)
                    {
                        player.HorizontalVelocity += (player.transform.forward * MovementInput.y + player.transform.right * player.TotalSpeedMultiplier * MovementInput.x) * jumpPlanarImpulse;
                    }

                }
                break;
            case MotionType.AnimatorController:
                break;
            default:
                break;
        }
        animator?.CrossFade("JumpUpward", 0.05f);
        currentJumpCount--;

    }

    public override void EnterState()
    {
        base.EnterState();
        ExecuteJump();
    }

    public void ResetJump(int newJumpCount = -1)
    {
        IsJumping = false;
        currentJumpCount = newJumpCount >= 0 ? newJumpCount : maxJumpCount;
    }
}