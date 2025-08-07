using LOGIYGames;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class RollActionContext : ActionContextBase
{
    [field: SerializeField] public bool CanRoll { get; set; } = true;
    public bool IsRolling { get => animator.GetBool(isRollingHash); private set => animator.SetBool(isRollingHash, value); }
    int isRollingHash = Animator.StringToHash("IsRolling");
    int RollHash = Animator.StringToHash("Roll");


    private void OnEnable()
    {
        Input.EvadeEvent.AddListener(PerformRoll);
    }

    private void OnDisable()
    {
        Input.EvadeEvent.RemoveListener(PerformRoll);
    }
    private void PerformRoll(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {

            case InputActionPhase.Performed:
                if (player.IsGrounded&& CanRoll&&!IsRolling)
                {
                    IsRolling = true;
                }
                break;
            default:
                break;
        }
    }
    protected override void Rotate()
    {
        return;
    }
    protected override void ChangeVelocity()
    {
        player.HorizontalVelocity = Vector3.zero;
    }
    private void OnAnimationEnd()
    {
        IsRolling = false;
    }

}