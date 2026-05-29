using LOGIYGames.Animation;
using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames
{
    public partial class AbilityController
    {
        public Ability CurrentAbility { get; private set; }
        private CharacterModule character;
        private CharacterAnimationModule animator;
        private CountdownTimer castTimer;
        public float CastingProgress => castTimer.Progress;
        public AbilityPhase Phase { get; private set; }
        public AbilityController(CharacterModule owner)
        {
            character = owner;
            animator = owner.GetComponent<CharacterAnimationModule>();
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            character.EventBus.Subscribe<CharacterAnimationEvent>(AnimationEventsHandler);
        }
        public void BeginAbility(Ability ability)
        {
            if (CurrentAbility != null) return;
            Phase = AbilityPhase.Started;
            CurrentAbility = ability;
            StartCasting();
        }
        private void StartCasting()
        {
            Phase = AbilityPhase.Casting;
            if (!string.IsNullOrEmpty(CurrentAbility.castingAnimation))
            {
                animator.PlayAnimation(CurrentAbility.castingAnimation);
            }
            castTimer = new CountdownTimer(CurrentAbility.castDuration);
            castTimer.OnTimerStop = StartExecution;
            castTimer.Start();
        }
        private void StartExecution()
        {
            Phase = AbilityPhase.Executing;
            animator.PlayAnimation(CurrentAbility.executionAnimation);
        }
        private void AnimationEventsHandler(CharacterAnimationEvent e)
        {
            switch (e.AnimationEventType)
            {
                case AnimationEventType.AbilityStarted:
                    OnAbilityStarted();
                    break;
                case AnimationEventType.AbilityActionStart:
                    OnAbilityActionStarted();
                    break;
                case AnimationEventType.AbilityActionEnd:
                    OnAbilityActionEnded();
                    break;
                case AnimationEventType.AbilityFinished:
                    OnAbilityFinished();
                    break;
            }
        }
        #region Events Handlers
        private void OnAbilityStarted()
        {
            Debug.Log(
                $"Ability Started: " +
                $"{CurrentAbility.name}");
        }
        private void OnAbilityActionStarted()
        {
            Debug.Log(
                $"Ability Action Started: " +
                $"{CurrentAbility.name}");
        }
        private void OnAbilityActionEnded()
        {
            Debug.Log(
                $"Ability Action Ended: " +
                $"{CurrentAbility.name}");
        }
        private void OnAbilityFinished()
        {
            Debug.Log(
                $"Ability Finished: " +
                $"{CurrentAbility.name}");

            CurrentAbility = null;

            Phase = AbilityPhase.Finished;
        }
        #endregion
        public bool IsFinished()
        {
            return Phase == AbilityPhase.Finished;
        }
        public void Reset()
        {
            Phase = AbilityPhase.None;
            CurrentAbility = null;
        }

    }
}
