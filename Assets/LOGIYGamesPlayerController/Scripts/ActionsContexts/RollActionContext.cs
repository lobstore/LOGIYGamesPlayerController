using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
public class RollActionContext : NetworkBehaviour, IActionContext
{
    [field: SerializeField] public MotionType MotionType { get; private set; }

    [Header("Component References")]
    [SerializeField] private Animator animator;
    private CharacterModule player;
    private PlayerCameraManager cameraManager;
    private PlayerMovementInputManager input;

    [Header("Jump Settings")]
    [SerializeField] private float rollDistance = 2f;
    [SerializeField] private float rollCooldown = 0.2f;
    [SerializeField] private int maxRollsCount = 1;
    [field: SerializeField] public bool CanRoll { get; set; } = true;
    // State Variables
    private CountdownTimer rollCooldownTimer;
    private int currentRollCount;
    public bool IsRolling { get => animator.GetBool(isRollingHash); private set => animator.SetBool(isRollingHash, value); }
    int isRollingHash = Animator.StringToHash("IsRolling");

    [field: SerializeField] public float Acceleration { get; private set; } = 2f;
    [field: SerializeField] public float Deceleration { get; private set; } = 25f;
    private void Awake()
    {
        InitializeComponents();
        InitializeRollSystem();
    }

    private void InitializeComponents()
    {
        player = GetComponent<CharacterModule>();
        cameraManager = GetComponent<PlayerCameraManager>();
    }

    private void InitializeRollSystem()
    {
        rollCooldownTimer = new CountdownTimer(rollCooldown);
        player.PlayerTimers.Add(rollCooldownTimer);
        currentRollCount = maxRollsCount;
    }

    private void OnEnable()
    {
        if (PlayerMovementInputManager.Instance == null)
        {
            Debug.LogWarning("PlayerMovementInputManager.Instance was not found");
            return;
        }

        input = PlayerMovementInputManager.Instance;
        input.Rolled.AddListener(Roll);
    }

    private void OnDisable()
    {
        if (input == null) return;
        input.Rolled.RemoveListener(Roll);
    }

    private void Roll()
    {
        if (!IsOwner) return;
        if (!player.IsGrounded) return;
        if (!CanRoll) return;
        if (IsRolling) return;
        IsRolling = true;
    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
        Move();
    }
    public void Move()
    {
        if (!cameraManager.IsFP || rollCooldownTimer.IsRunning) return;
    }
    private void Rotate()
    {
        if (input.MovementInput.magnitude == 0) return;

        float targetAngle = Mathf.Atan2(input.MovementInput.x, input.MovementInput.y) * Mathf.Rad2Deg
                          + cameraManager.CurentCameraController.CameraTransform.eulerAngles.y;



        player.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }
    private void ExecuteRoll()
    {
        Rotate();
    }

    public void EnterState()
    {
        ExecuteRoll();
        player.Acceleration = Acceleration;
        player.Deceleration = Deceleration;
    }

    public void ExitState()
    {
        //player.InternalSpeedMultiplier = 0f;
    }
    private void OnAnimationEnd()
    {
        IsRolling = false;
    }

    public void ResetRoll(int newRollsCount = -1)
    {
        IsRolling = false;
        currentRollCount = newRollsCount >= 0 ? newRollsCount : maxRollsCount;
    }

}