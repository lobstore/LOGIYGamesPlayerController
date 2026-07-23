using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Data;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageEffect : IEffect
    {
        private readonly DamageData damage;

        public DamageEffect(DamageData damage)
        {
            this.damage = damage;

        }

        public void Apply(AbilityContext context)
        {
            if (context.Target == null)
                return;

            HealthController health = context.Target.GetComponent<HealthController>();

            if (health == null)
                return;

            health.TakeDamage(new DamageContext()
            {
                Damage = damage,
                Source = context.Source,
                Target = context.Target
            });

            Debug.Log(
                $"{context.Source.name} dealt " +
                $"{damage} to " +
                $"{context.Target.name}");
        }

        public void Cancel()
        {

        }
    }
}
