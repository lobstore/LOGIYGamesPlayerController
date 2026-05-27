using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(menuName = "Effects/Inatant Damage Effect")]
    public class DamageEffectData : EffectFactory
    {
        public float damage;

        public override IEffect CreateEffect()
        {
            return new DamageEffect(damage);
        }
    }
}
