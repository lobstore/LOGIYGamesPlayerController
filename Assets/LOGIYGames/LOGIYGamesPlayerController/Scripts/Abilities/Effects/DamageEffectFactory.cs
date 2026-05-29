using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "Damage Effect", menuName = "Ability/Effects/Damage")]
    public class DamageEffectFactory : EffectFactory
    {
        public float damage;

        public override IEffect CreateEffect()
        {
            return new DamageEffect(damage);
        }
    }
}
