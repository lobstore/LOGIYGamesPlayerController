using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Timers;
using R3;
using System;
using UnityEngine;
namespace LOGIYGames
{
    public class Ability
    {
        public AbilityData Data { get; set; }
        public CountdownTimer CooldownTimer { get; set; }
        AbilityController controller;
        IDisposable executionTimerSubscribtion;
        private CountdownTimer castTimer;
        private CountdownTimer executionTimer;
        public AbilityPhase Phase { get; private set; }
        int nextTargetingEventIndex = 0;
        int nextAnimationEventIndex = 0;
        public Ability(AbilityController abilityController, AbilityData data)
        {
            Data = data;
            controller = abilityController;
            CooldownTimer = new CountdownTimer(Data.cooldown);
            castTimer = new CountdownTimer(Data.castDuration);
            executionTimer = new CountdownTimer(Data.executionDuration);


            castTimer.OnTimerStop = StartExecutionPhase;
            executionTimer.OnTimerStop = StartCooldownPhase;
            executionTimer.OnTimerStart += () =>
            {
                executionTimerSubscribtion = executionTimer.CurrentTime.Subscribe(currentTime =>
                {
                    while (nextTargetingEventIndex < Data.TargetingFactories.Count && executionTimer.ElapsedTime >= Data.TargetingFactories[nextTargetingEventIndex].EventTime)
                    {
                        OnTargetingStarted(Data.TargetingFactories[nextTargetingEventIndex]);
                        nextTargetingEventIndex++;
                    }
                });
                executionTimerSubscribtion = executionTimer.CurrentTime.Subscribe(currentTime =>
                {
                    while (nextAnimationEventIndex < Data.Animations.Count && executionTimer.ElapsedTime >= Data.Animations[nextAnimationEventIndex].EventTime)
                    {
                        OnAnimationStarted(Data.Animations[nextAnimationEventIndex]);
                        nextAnimationEventIndex++;
                    }
                });
            };
            CooldownTimer.OnTimerStop += () =>
            {
                Phase = AbilityPhase.Ready;
          
            };
            Phase = AbilityPhase.Ready;
        }

        public void StartCooldownPhase()
        {
            Phase = AbilityPhase.Cooldown;
            nextTargetingEventIndex = 0;
            nextAnimationEventIndex = 0;
            executionTimerSubscribtion.Dispose();
            CooldownTimer.Start();
        }

        public void Start()
        {
            ResetTimers();
            Phase = AbilityPhase.Started;
            StartCastingPhase();
        }

        public void StartExecutionPhase()
        {
            Phase = AbilityPhase.Executing;

            executionTimer.Start();
        }

        private void StartCastingPhase()
        {
            if (!string.IsNullOrEmpty(Data.castingAnimation))
            {
                controller.Animator.CrossFade(Data.castingAnimation, 0.1f);
            }
            Phase = AbilityPhase.Casting;
            castTimer.Start();
        }

        private void ResetTimers()
        {
            castTimer.Reset();
            executionTimer.Reset();
            CooldownTimer.Reset();
        }

        private void OnTargetingStarted(TargetingTimedEvent evt)
        {
            var targetingStrategy = evt.TargetingFactory.Create();

           
            targetingStrategy.Start(new AbilityContext
            {
                Source = controller.gameObject,
                Target = null
            });

        }
        private void OnAnimationStarted(AnimationTimedEvent e)
        {
            controller.Animator.applyRootMotion = e.animationData.UseRootMotion;
            controller.Animator.SetFloat("MotionSpeed", e.animationData.MotionSpeed);
            if (!string.IsNullOrEmpty(e.animationData.AnimationName))
            {
                controller.Animator.CrossFade(e.animationData.AnimationName, e.animationData.CrossFade);
            }
        }


    }
}
