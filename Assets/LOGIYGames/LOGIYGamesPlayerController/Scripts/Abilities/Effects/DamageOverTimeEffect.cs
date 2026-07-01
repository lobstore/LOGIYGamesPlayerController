using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageOverTimeEffect : IEffect
    {
        private readonly float duration;

        private readonly float tickInterval;

        private readonly float damagePerTick;

        private IntervalTimer timer;

        private AbilityContext context;

        public DamageOverTimeEffect(
            float duration,
            float tickInterval,
            float damagePerTick)
        {
            this.duration = duration;
            this.tickInterval = tickInterval;
            this.damagePerTick = damagePerTick;
        }

        public void Apply(
            AbilityContext context)
        {
            this.context = context;

            timer =
                new IntervalTimer(
                    duration,
                    tickInterval);

            timer.OnInterval = OnTick;

            timer.OnTimerStop = Cleanup;

            timer.Start();
        }

        private void OnTick()
        {
            if (context.Target == null)
            {
                Cancel();
                return;
            }

            Health health =
                context.Target
                    .GetComponent<Health>();

            if (health == null)
                return;

            health.ApplyDamage(
                damagePerTick);

            Debug.Log(
                $"DOT dealt " +
                $"{damagePerTick} to " +
                $"{context.Target.name}");
        }

        public void Cancel()
        {
            timer?.Stop();

            Cleanup();
        }

        private void Cleanup()
        {
            timer = null;
        }
    }

}
