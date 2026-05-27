using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(menuName = "Effects/Damage Over Time")]
    public class DamageOverTimeEffectData : EffectFactory
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
