using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
public class AttackActionContext : NetworkBehaviour, IActionContext
{
    [field: SerializeField] public MotionType MotionType {  get; private set; }
    CharacterModule player;
    [SerializeField] Animator animator;
    private PlayerInputsManager input;
    [SerializeField] private WeaponItem currentWeapon;
    private CombatControllerModule combatController;
    public bool IsAttackRequested { get; set; }
    public bool InContext { get; set; }

    [field: SerializeField] public float AirbornAcceleration { get; private set; } = 2f;
    [field: SerializeField] public float AirbornDeceleration { get; private set; } = 3f;
    private void Awake()
    {
        player = GetComponent<CharacterModule>();
        combatController = GetComponent<CombatControllerModule>();

    }
    private void OnEnable()
    {
        input = PlayerInputsManager.Instance;
        if (input == null)
        {
            Debug.LogWarning("PlayerMovementInputManager.Instance was not found");
        }
        input.Attacked.AddListener(Attack);
    }
    private void OnDisable()
    {
        input.Attacked.RemoveListener(Attack);
    }
    private void Attack()
    {
        if (!IsOwner) return;
        IsAttackRequested = true;
        if (InContext)
        {
            combatController.PerformAttack(currentWeapon);
        }
    }
    public void OnFixedUpdate()
    {
        if (!IsOwner) return;
        if (!combatController.IsAttacking)
        {
            IsAttackRequested = false;
        }

    }
    public void OnUpdate()
    {
        if (!IsOwner) return;
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
        combatController.PerformAttack(currentWeapon);
        player.InternalSpeedMultiplier = 0;
        InContext = true;
        if (!player.IsGrounded)
        {
            player.Acceleration = AirbornAcceleration;
            player.Deceleration = AirbornDeceleration;
        }
    }

    public void ExitState()
    {
        player.InternalSpeedMultiplier = 0;
        combatController.InterroptAttack();
        InContext = false;
    }
}