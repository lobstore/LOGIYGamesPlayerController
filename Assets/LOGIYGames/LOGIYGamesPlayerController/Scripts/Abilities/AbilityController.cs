using LOGIYGames.Animation;
using LOGIYGames.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] List<Ability> abilities;
        [SerializeField] GameObject Target;

        Ability Current;

        CharacterAnimationModule animator;

        CountdownTimer castingTimer;

        private void Awake()
        {
            animator = GetComponent<CharacterAnimationModule>();

        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Cast(abilities[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                Cast(abilities[1]);
            }
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
