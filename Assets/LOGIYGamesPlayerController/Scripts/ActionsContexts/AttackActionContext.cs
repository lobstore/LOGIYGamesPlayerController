using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
public class AttackActionContext : NetworkBehaviour, IActionContext
{
    public MotionType MotionType => throw new System.NotImplementedException();
    CharacterModule player;
    Animator animator;
    private MouseInputManager input;
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
        animator = GetComponent<Animator>();

    }
    private void OnEnable()
    {
        input = MouseInputManager.Instance;
        if (input == null)
        {
            Debug.LogWarning("PlayerMovementInputManager.Instance was not found");
        }
        input.LCMPressed.AddListener(Attack);
    }
    private void OnDisable()
    {
        input.LCMPressed.RemoveListener(Attack);
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
    public void OnUpdate()
    {
        if (!IsOwner) return;
        if (!combatController.IsAttacking)
        {
            IsAttackRequested = false;
        }

    }
    public void EnterState()
    {
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