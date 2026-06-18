using LOGIYGames.Timers;
using System;
using UnityEngine;
using R3;
namespace LOGIYGames
{
    public class Ability
    {
        public AbilityData Data;
        [Header("Targeting")]

        public CountdownTimer CooldownTimer { get; set; }
        public AbilityPhase Phase { get; private set; }
        public Ability(AbilityData data)
        {
            Data = data;
            CooldownTimer = new(Data.CooldownDuration);
            CooldownTimer.OnTimerStart += () =>
            {
                Phase = AbilityPhase.Cooldown;
            };
            CooldownTimer.OnTimerStop += () =>
            {
                Phase = AbilityPhase.Ready;
            };
        }

        public void Target(AbilityController abilityController)
        {
            Phase = AbilityPhase.Targeting;
            if (Data.TargetingStrategy != null)
            {
                var strat = Data.TargetingStrategy.Create();

                strat.Start(this, abilityController);
            }
        }
        public void ApplyEffects(GameObject target)
        {
            foreach (var item in Data.Effects)
            {
                var effec = item.CreateEffect();
                //target.ApplyEffect();
            }
        }
        public void ResetCooldown()
        {
            CooldownTimer.CurrentTime.Value = 0;
        }
        public void SetCooldown()
        {
            CooldownTimer.Start();
        }
    }
}
