using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilityController : MonoBehaviour
    {
        public Ability CurrentAbility { get; private set; } = null;
        public List<AbilityData> abilities = new();
        public List<Ability> Abilities { get; private set; } = new();
        public Animator Animator { get; private set; }

        private void Awake()
        {
            foreach (var item in abilities)
            {
                Abilities.Add(new Ability(this, item));
            }
        }
        private void Start()
        {
            Animator = GetComponent<Animator>();
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
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetAbility(Abilities[0]);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetAbility(Abilities[1]);
            }
        }

        public bool IsFinished()
        {
            return CurrentAbility.Phase == AbilityPhase.Cooldown;
        }

        public void Exit()
        {
            Animator.SetFloat("MotionSpeed", 1);
            CurrentAbility = null;

        }
    }
}
