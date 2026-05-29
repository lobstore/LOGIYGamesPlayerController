using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(
        fileName = "DOT Effect",
        menuName = "Ability/Effects/DOT")]
    public class DamageOverTimeEffectFactory
        : EffectFactory
    {
        public float duration;

        public float tickInterval;

        public float damagePerTick;

        public override IEffect CreateEffect()
        {
            return new DamageOverTimeEffect(
                duration,
                tickInterval,
                damagePerTick);
        }
    }


}
