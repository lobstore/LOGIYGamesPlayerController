using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageOverTimeEffect : IEffect
    {
        private readonly float duration;

        private readonly float tickInterval;

        private readonly DamageContext damagePerTick;

        private IntervalTimer timer;

        private AbilityContext context;

        public DamageOverTimeEffect(
            float duration,
            float tickInterval,
            DamageContext damagePerTick)
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

            HealthController health =
                context.Target
                    .GetComponent<HealthController>();

            if (health == null)
                return;

            health.TakeDamage(
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
