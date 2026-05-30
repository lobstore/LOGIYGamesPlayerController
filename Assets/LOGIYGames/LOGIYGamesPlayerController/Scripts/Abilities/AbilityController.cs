using LOGIYGames.Animation;
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

        IDisposable timerSubscribtion;

        public Ability CurrentAbility { get; private set; }

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

            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            //character.EventBus.Subscribe<AbilityTimelineEvent>(e => { TriggerTimedEvent(e.AbilityEventType); });

        }


        public void BeginAbility(
            Ability ability,
            GameObject target = null)
        {
            if (CurrentAbility != null)
                return;

            CurrentAbility = ability;

            Phase = AbilityPhase.Started;
            castTimer = new CountdownTimer(CurrentAbility.castDuration);

            executionTimer = new CountdownTimer(CurrentAbility.executionDuration);

            int nextEventIndex;

            executionTimer.OnTimerStart += () =>
            {
                nextEventIndex = 0;

                timerSubscribtion = executionTimer.CurrentTime.Subscribe(currentTime =>
                {
                    while (nextEventIndex < CurrentAbility.TimedEvents.Count && executionTimer.ElapsedTime >= CurrentAbility.TimedEvents[nextEventIndex].EventTime)
                    {
                        TriggerTimedEvent(CurrentAbility.TimedEvents[nextEventIndex]);
                        nextEventIndex++;
                    }
                });
            };
            executionTimer.OnTimerStop += () =>
            {
                nextEventIndex = 0;
                timerSubscribtion.Dispose();
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

        private void TriggerTimedEvent(AbilityTimedEvent e)
        {
            switch (e.AbilityEventType)
            {
                case AbilityEventType.Started:
                    OnAbilityStarted(e);
                    break;

                case AbilityEventType.ActionStart:
                    OnAbilityActionStarted(e);
                    break;

                case AbilityEventType.ActionEnd:
                    OnAbilityActionEnded(e);
                    break;

                case AbilityEventType.Finished:
                    OnAbilityFinished(e);
                    break;
            }
        }

        #region Event Handlers

        private void OnAbilityStarted(AbilityTimedEvent e)
        {
            Debug.Log(
                $"Ability Started: {CurrentAbility.name}");
        }

        private void OnAbilityActionStarted(AbilityTimedEvent e)
        {
            animator.applyRootMotion = e.UseRootMotion;
            animator.SetFloat("MotionSpeed", e.MotionSpeed);
            if (!string.IsNullOrEmpty(e.animationName))
            {
                animator.CrossFade(e.animationName, e.CrossFade);
            }

            Debug.Log(
                $"Ability Action Started: {CurrentAbility.name}");
        }

        private void OnAbilityActionEnded(AbilityTimedEvent e)
        {
   
            Debug.Log(
                $"Ability Action Ended: {CurrentAbility.name}");
        }

        private void OnAbilityFinished(AbilityTimedEvent e)
        {
            Debug.Log(
                $"Ability Finished: {CurrentAbility.name}");
            animator.SetFloat("MotionSpeed", 1);
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
            animator.SetFloat("MotionSpeed", 1);
            CurrentAbility = null;

            Phase = AbilityPhase.None;
        }
    }
}
