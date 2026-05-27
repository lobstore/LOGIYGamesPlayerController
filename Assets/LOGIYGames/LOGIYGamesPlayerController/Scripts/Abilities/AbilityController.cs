using LOGIYGames.Animation;
using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilityController
    {
        [SerializeField] List<Ability> abilities;
        [SerializeField] GameObject Target;

        Ability Current;

        CharacterModule character;

        CharacterAnimationModule animator;

        CountdownTimer castingTimer;

        public AbilityController(CharacterModule owner)
        {
            character = owner;
            animator = owner.GetComponent<CharacterAnimationModule>();
        }
        public void Cast(Ability skill)
        {

            if (Current == null)
            {
                Current = skill;
            }
            else
            {
                return;
            }
            Debug.Log("Casted" + $"{skill.name}");
            castingTimer = new CountdownTimer(Current.castDuration);
            castingTimer.OnTimerStart = () =>
            {
                if (!string.IsNullOrEmpty(Current.castingAnimationName))
                {

                    animator.PlayAnimation(Current.castingAnimationName);
                }
            };
            castingTimer.OnTimerStop = () =>
            {
                if (!string.IsNullOrEmpty(Current.executionAnimationName))
                {

                    animator.PlayAnimation(Current.executionAnimationName);
                }
                foreach (var effectData in Current.effects)
                {

                    IEffect effect = effectData.CreateEffect();

                    effect.Apply(Target);
                }
                Current = null;
            };
            castingTimer.Start();
        }
    }
}
