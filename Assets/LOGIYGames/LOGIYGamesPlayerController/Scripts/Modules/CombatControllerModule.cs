using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames
{
    public class CombatControllerModule : NetworkModuleBase
    {
        Animator animator;
        int hashComboIndex = Animator.StringToHash("ComboIndex");
        int hashIsAttackingBool = Animator.StringToHash("IsAttacking");
        int hashAttackTrigger = Animator.StringToHash("Attack");
        public UnityEvent AttackEnded { get; private set; } = new UnityEvent();
        public UnityEvent AttackStarted { get; private set; } = new UnityEvent();
        public int ComboIndex
        {
            get => animator.GetInteger(hashComboIndex);
            set => animator.SetInteger(hashComboIndex, value);
        }
        public bool IsAttacking
        {
            get => animator.GetBool(hashIsAttackingBool);
            set => animator.SetBool(hashIsAttackingBool, value);
        }
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void SetWeaponDependedAnimator(AnimatorOverrideController newOVAnimator)
        {
            animator.runtimeAnimatorController = newOVAnimator;
        }
        public void PerformAttack(WeaponItem weapon, int comboIndex = 0)
        {
            ComboIndex = comboIndex;
            if (!IsAttacking)
            {
                IsAttacking = true;
            }
            AttackStarted.Invoke();

            animator.SetTrigger(hashAttackTrigger);
        }

        public void InterroptAttack()
        {
            IsAttacking = false;
            animator.ResetTrigger(hashAttackTrigger);
        }
    }
}