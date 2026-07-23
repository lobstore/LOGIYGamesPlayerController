using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageEffect : IEffect
    {
        private readonly DamageContext damage;

        public DamageEffect(DamageContext damage)
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

            health.TakeDamage(damage);

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
