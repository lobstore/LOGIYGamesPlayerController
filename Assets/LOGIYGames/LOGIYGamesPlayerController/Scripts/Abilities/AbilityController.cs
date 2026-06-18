using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class AbilityController : MonoBehaviour
    {
        private CharacterModule characterModule;
        public Ability CurrentAbility { get; private set; } = null;
        [field: SerializeField] public List<AbilityFactory> AbilityFactories { get; private set; } = new();
        public List<Ability> Abilities { get; private set; } = new();
        public UnityEvent OnCastFinished { get; private set; } = new();
        public UnityEvent OnCastStarted { get; private set; } = new();
        public CountdownTimer CastTimer { get; private set; }
        public CountdownTimer ExecutionTimer { get; private set; }

        public Animator Animator { get; private set; }
        public TargetingManager TargetingManager;
        private void Awake()
        {
            Animator = GetComponent<Animator>();
            foreach (var factory in AbilityFactories)
            {
                Abilities.Add(factory.Create());
            }
            CastTimer = new CountdownTimer();
            ExecutionTimer = new CountdownTimer();
            CastTimer.OnTimerStart += () =>
            {
                OnCastStarted?.Invoke();
                Animator.SetFloat("MotionSpeed", CurrentAbility.Data.castingAnimation.MotionSpeed);
                Animator.CrossFade(CurrentAbility.Data.castingAnimation.AnimationName, CurrentAbility.Data.castingAnimation.CrossFade);
            };
            CastTimer.OnTimerStop += () =>
            {
                if (CastTimer.IsFinished)
                {
                    OnCastFinished?.Invoke();
                    Animator.SetFloat("MotionSpeed", 1);
                    ExecutionTimer.Start();
                }
            };
            ExecutionTimer.OnTimerStart += () =>
            {
                CurrentAbility.Target(this);
            };
            ExecutionTimer.OnTimerStop += () =>
            {
                Animator.SetFloat("MotionSpeed", 1);

                CurrentAbility = null;
            };
        }
        public void SetAbility(Ability ability)
        {
            if (CurrentAbility == null && ability.Phase == AbilityPhase.Ready)
            {
                CurrentAbility = ability;
            }
        }
        public void BeginAbility()
        {
            if (CurrentAbility == null) { return; }
            CastTimer.Reset(CurrentAbility.Data.CastDuration);
            ExecutionTimer.Reset(CurrentAbility.Data.ExecutionDuration);
            StartCasting();
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
        public void StartCasting()
        {
            CastTimer.Start();
        }
        public void StopCasting()
        {
            CastTimer.Stop();
        }
        public void ResetCasting(float newCastTime = 0)
        {
            CastTimer.Reset(newCastTime);
        }
    }
}
