using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(CharacterModule))]
public class JumpActionContext : NetworkBehaviour, IActionContext
{
    [Header("Component References")]
    [SerializeField] private Animator animator;
    private CharacterModule player;
    private PlayerCameraManager cameraManager;
    private PlayerInputsManager input;

    [Header("Jump Settings")]
    [SerializeField] private float jumpVerticalImpulse = 1.5f;
    [SerializeField] private float jumpPlanarImpulse = 1f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private int maxJumpCount = 2;
    [field: SerializeField] public bool CanJump { get; set; } = true;

    [Header("Movement Settings")]
    [SerializeField] private float turnSmoothTime = 10f;
    private float turnSmoothVelocity;
    [field: SerializeField] public float Acceleration { get; private set; } = 0f;
    [field: SerializeField] public float Deceleration { get; private set; } = 1f;
    [field: SerializeField] public float InternalSpeedMultiplier { get; private set; } = 1f;
    [field: SerializeField] public MotionType MotionType { get; private set; }
    // State Variables
    private CountdownTimer jumpCooldownTimer;
    private int currentJumpCount;
    public bool IsJumping { get; private set; }


    private void Awake()
    {
        InitializeComponents();
        InitializeJumpSystem();
    }

    private void InitializeComponents()
    {
        player = GetComponent<CharacterModule>();
        cameraManager = GetComponent<PlayerCameraManager>();
    }

    private void InitializeJumpSystem()
    {
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        player.PlayerTimers.Add(jumpCooldownTimer);
        currentJumpCount = maxJumpCount;
    }

    private void OnEnable()
    {
        if (PlayerInputsManager.Instance == null)
        {
            Debug.LogWarning("PlayerMovementInputManager.Instance was not found");
            return;
        }
        jumpCooldownTimer.Reset(jumpCooldown);
        input = PlayerInputsManager.Instance;
        input.Jumped.AddListener(Jump);
        jumpCooldownTimer.OnTimerStart += StartJump;
        jumpCooldownTimer.OnTimerStop += StopJump;

    }

    private void OnDisable()
    {
        if (input == null) return;
        input.Jumped.RemoveListener(Jump);
        jumpCooldownTimer.OnTimerStart -= StartJump;
        jumpCooldownTimer.OnTimerStop -= StopJump;

    }

    private void Jump()
    {
        if (!IsOwner || !CanJump || currentJumpCount <= 0 || jumpCooldownTimer.IsRunning) return;

        jumpCooldownTimer.Start();
    }
    private void StartJump()
    {
        IsJumping = true;

    }
    private void StopJump()
    {
        IsJumping = false;
    }
    public void OnFixedUpdate()
    {
        if (!IsOwner) return;
        Move();
    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
    }
    private void Move()
    {
        if (!cameraManager.IsFP) return;
        AlignWithCamera();
    }

    private void AlignWithCamera()
    {
        float targetAngle = cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(
            player.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            1 / (turnSmoothTime * 4));

        player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void ExecuteJump()
    {
        player.VerticalVelocity = Mathf.Sqrt(jumpVerticalImpulse * -2f * Physics.gravity.y);
        if (input.MovementInput.magnitude > 0)
        {
            if (!cameraManager.IsFP)
            {

                player.HorizontalVelocity += player.transform.forward * player.TotalSpeedMultiplier * input.MovementInput.magnitude * jumpPlanarImpulse;
            }
            else
            {
                player.HorizontalVelocity += (player.transform.forward * input.MovementInput.y + player.transform.right * player.TotalSpeedMultiplier * input.MovementInput.x) *player.CurrentSpeed* jumpPlanarImpulse;
 
            }
        }
        animator?.CrossFade("JumpUpward", 0.05f);
        currentJumpCount--;
    }

    public void EnterState()
    {
        if (MotionType == MotionType.AnimatorController)
        {
            animator.applyRootMotion = true;
        }
        else
        {
            animator.applyRootMotion = false;
        }
        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;
        player.InternalSpeedMultiplier = 1f;
        ExecuteJump();
    }


    public void ExitState()
    {
        // Cleanup logic can be added here if needed
    }

    public void ResetJump(int newJumpCount = -1)
    {
        IsJumping = false;
        currentJumpCount = newJumpCount >= 0 ? newJumpCount : maxJumpCount;
    }
}