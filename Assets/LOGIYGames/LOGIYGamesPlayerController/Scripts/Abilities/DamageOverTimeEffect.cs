using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Data;
using LOGIYGames.Timers;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageOverTimeEffect : IEffect
    {
        private float duration;
        private float tickInterval;
        private float damagePerTick;

        private IntervalTimer timer;

        private GameObject target;

        public DamageOverTimeEffect(
            float duration,
            float tickInterval,
            float damagePerTick)
        {
            this.duration = duration;
            this.tickInterval = tickInterval;
            this.damagePerTick = damagePerTick;
        }

        public void Apply(GameObject target)
        {
            this.target = target;

            timer = new IntervalTimer(duration, tickInterval);

            timer.OnInterval = OnInterval;
            timer.OnTimerStop = OnStop;

            timer.Start();
        }

        private void OnInterval()
        {
            if (target == null)
            {
                Cancel();
                return;
            }

            target
                .GetComponent<HealthModule>()
                .ApplyDamage(damagePerTick);

            Debug.Log($"DOT dealt {damagePerTick} to {target.name}");
        }

        private void OnStop()
        {
            Cleanup();
        }

        public void Cancel()
        {
            timer?.Stop();
            Cleanup();
        }

        private void Cleanup()
        {
            timer = null;
            target = null;
        }
    }
}
