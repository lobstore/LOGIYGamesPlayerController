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
    private void FixedUpdate()
    {
        IsAttackRequested = Character.AttackPressed;

        if (InContext)
        {
            combatController.PerformAttack(currentWeapon);
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
        Character.AttackPressed = false;
        base.EnterState();
        combatController.PerformAttack(currentWeapon);
        Character.InternalSpeedMultiplier = 0;
        InContext = true;
    }

    public override void ExitState()
    {
        base.ExitState();
        Character.InternalSpeedMultiplier = 0;
        combatController.InterroptAttack();
        InContext = false;
    }
}