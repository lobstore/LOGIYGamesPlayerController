using LOGIYGames;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterModule))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SensorsModule))]
public class CrouchActionContext : LocomotionActionContext
{

    [Header("Movement Settings")]
    [SerializeField] private float crouchHeightMultiplier = 0.5f;
    [Header("Component References")]
    private CharacterController characterController;

    public bool IsCrouching => sensors.IsObstacleAbove || IsCrouchingPressed;
    private bool IsCrouchingPressed;

    public float CrouchHeight { get; private set; }
    public float StandingHeight { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeComponents();
        InitializeHeightValues();
    }
    private void OnEnable()
    {
        Input.CrouchEvent.AddListener(PerformCrouch);
    }

    private void PerformCrouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                IsCrouchingPressed = true;
                break;
            case InputActionPhase.Canceled:
                IsCrouchingPressed = false;
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        Input.CrouchEvent.RemoveListener(PerformCrouch);
    }
    private void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void InitializeHeightValues()
    {
        StandingHeight = characterController.height;
        CrouchHeight = StandingHeight * crouchHeightMultiplier;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void EnterState()
    {
        base.EnterState();
        player.Height = CrouchHeight;
    }
    public override void ExitState()
    {
        base.ExitState();
        player.Height = StandingHeight;
    }

}