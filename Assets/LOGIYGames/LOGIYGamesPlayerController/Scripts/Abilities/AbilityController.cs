using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilityController
    {
        public Ability CurrentAbility { get; private set; } = null;

        public CharacterModule character { get; private set; }
        public Animator animator { get; private set; }

        public AbilityController(CharacterModule owner)
        {
            character = owner;
            animator = owner.GetComponent<Animator>();
        }

        public void SetAbility(Ability ability)
        {
            if (CurrentAbility == null && ability.Phase != AbilityPhase.Cooldown)
            {
                CurrentAbility = ability;
            }
        }
        public void BeginAbility()
        {
            CurrentAbility.Start();
        }


        public bool IsFinished()
        {
            return CurrentAbility.Phase == AbilityPhase.Cooldown;
        }

        public void Exit()
        {
            animator.SetFloat("MotionSpeed", 1);
            CurrentAbility = null;

        }
    }
}
