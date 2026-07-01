using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageEffect : IEffect
    {
        private readonly float damage;

        public DamageEffect(float damage)
        {
            this.damage = damage;
        }

        public void Apply(AbilityContext context)
        {
            if (context.Target == null)
                return;

            Health health =
                context.Target
                    .GetComponent<Health>();

            if (health == null)
                return;

            health.ApplyDamage(damage);

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
