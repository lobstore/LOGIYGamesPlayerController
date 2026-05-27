using LOGIYGames.CharacterCore;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class DamageEffect : IEffect
    {
        private float damage;

        public DamageEffect(float damage)
        {
            this.damage = damage;
        }

        public void Apply(GameObject target)
        {
            target.GetComponent<HealthModule>().ApplyDamage(damage);
            Debug.Log($"dealt {damage} damage to {target.name}");
        }

        public void Cancel()
        {

        }
    }
}
