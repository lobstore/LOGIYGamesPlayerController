using LOGIYGames;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackActionContext : ActionContextBase
{
    [SerializeField] private WeaponItem currentWeapon;
    private CombatControllerModule combatController;
    public bool IsAttackRequested { get; set; }
    public bool InContext { get; set; }
    protected override void Awake()
    {
        base.Awake();
        combatController = GetComponent<CombatControllerModule>();

    }
    private void OnEnable()
    {
        Input.AttackEvent.AddListener(Attack);
    }
    private void OnDisable()
    {
        Input.AttackEvent.RemoveListener(Attack);
    }
    private void Attack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                IsAttackRequested = true;
                if (InContext)
                {
                    combatController.PerformAttack(currentWeapon);
                }
                break;
            case InputActionPhase.Canceled:
                break;
            default:
                break;
        }
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (!combatController.IsAttacking)
        {
            IsAttackRequested = false;
        }

    }
    public override void EnterState()
    {
        base.EnterState();
        combatController.PerformAttack(currentWeapon);
        player.InternalSpeedMultiplier = 0;
        InContext = true;
    }

    public override void ExitState()
    {
        base.ExitState();
        player.InternalSpeedMultiplier = 0;
        combatController.InterroptAttack();
        InContext = false;
    }
}