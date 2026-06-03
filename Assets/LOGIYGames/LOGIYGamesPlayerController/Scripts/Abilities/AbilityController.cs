using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using LOGIYGames.Timers;
using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilityController
    {

        IDisposable executionTimerSubscribtion;

        public AbilityData CurrentAbility { get; private set; } = null;

        private CharacterModule character;
        private Animator animator;

        private CountdownTimer castTimer;
        private CountdownTimer executionTimer;
        public float CastingProgress => castTimer?.Progress ?? 0f;

        public AbilityPhase Phase { get; private set; }

        public AbilityController(CharacterModule owner)
        {
            character = owner;
            animator = owner.GetComponent<Animator>();
        }

        public void SetAbility(AbilityData abilityData)
        {
            if (CurrentAbility == null)
            {
                CurrentAbility = abilityData;
            }
        }
        public void BeginAbility()
        {

            Phase = AbilityPhase.Started;
            castTimer = new CountdownTimer(CurrentAbility.castDuration);

            executionTimer = new CountdownTimer(CurrentAbility.executionDuration);

            int nextTargetingEventIndex;
            int nextAnimationEventIndex;

            executionTimer.OnTimerStart += () =>
            {
                nextTargetingEventIndex = 0;
                nextAnimationEventIndex = 0;
                executionTimerSubscribtion = executionTimer.CurrentTime.Subscribe(currentTime =>
                {
                    while (nextTargetingEventIndex < CurrentAbility.TargetingFactories.Count && executionTimer.ElapsedTime >= CurrentAbility.TargetingFactories[nextTargetingEventIndex].EventTime)
                    {
                        OnTargetingStarted(CurrentAbility.TargetingFactories[nextTargetingEventIndex]);
                        nextTargetingEventIndex++;
                    }
                });
                executionTimerSubscribtion = executionTimer.CurrentTime.Subscribe(currentTime =>
                {
                    while (nextAnimationEventIndex < CurrentAbility.Animations.Count && executionTimer.ElapsedTime >= CurrentAbility.Animations[nextAnimationEventIndex].EventTime)
                    {
                        OnAnimationStarted(CurrentAbility.Animations[nextAnimationEventIndex]);
                        nextAnimationEventIndex++;
                    }
                });
            };
            executionTimer.OnTimerStop += () =>
            {
                nextTargetingEventIndex = 0;
                executionTimerSubscribtion.Dispose();
                Phase = AbilityPhase.Finished;
            };

            castTimer.OnTimerStop = StartExecution;
            StartCasting();
        }

        private void StartCasting()
        {
            Phase = AbilityPhase.Casting;

            if (!string.IsNullOrEmpty(CurrentAbility.castingAnimation))
            {
                animator.CrossFade(CurrentAbility.castingAnimation, 0.1f);
            }
            castTimer.Start();
        }

        private void StartExecution()
        {
            Phase = AbilityPhase.Executing;
            executionTimer.Start();
        }

        private void OnTargetingStarted(TargetingTimedEvent evt)
        {
            var targetingStrategy = evt.TargetingFactory.Create(evt.vFXData);
            targetingStrategy.Start(new AbilityContext
            {
                Source = character.gameObject,
                Target = null
            });

        }

        private void OnAnimationStarted(AnimationTimedEvent e)
        {
            animator.applyRootMotion = e.animationData.UseRootMotion;
            animator.SetFloat("MotionSpeed", e.animationData.MotionSpeed);
            if (!string.IsNullOrEmpty(e.animationData.AnimationName))
            {
                animator.CrossFade(e.animationData.AnimationName, e.animationData.CrossFade);
            }
        }

        public bool IsFinished()
        {
            return Phase == AbilityPhase.Finished;
        }

        public void Reset()
        {
            animator.SetFloat("MotionSpeed", 1);
            CurrentAbility = null;

            Phase = AbilityPhase.None;
        }
    }
}
