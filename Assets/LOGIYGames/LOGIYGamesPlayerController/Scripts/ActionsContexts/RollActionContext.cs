using LOGIYGames;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    public class RollActionContext : GroundedActionContext
    {
        [field: SerializeField] public bool CanRoll { get; set; } = true;
        public bool IsRolling { get => animator.GetBool(isRollingHash); private set => animator.SetBool(isRollingHash, value); }
        int isRollingHash = Animator.StringToHash("IsRolling");
        int RollHash = Animator.StringToHash("Roll");

        private void FixedUpdate()
        {
            if (Character.EvadePressed && Sensors.IsGrounded && CanRoll && !IsRolling) {
                IsRolling = true;
            }
        }

        protected override void Rotate()
        {
            return;
        }
        protected override void ChangeVelocity()
        {
            Character.HorizontalVelocity = Vector3.zero;
        }
        private void OnAnimationEnd()
        {
            IsRolling = false;
        }
        public override void EnterState()
        {
            Character.EvadePressed = false;
            base.EnterState();
        }
    }
}