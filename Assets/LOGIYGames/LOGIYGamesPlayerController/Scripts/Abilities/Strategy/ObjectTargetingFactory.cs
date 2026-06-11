using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ObjectStrategy", menuName = "Targeting/Factory/Object")]
    public class ObjectTargetingFactory : TargetingFactory
    {
        public override TargetingStrategy Create()
        {
            var effects = new List<IEffect>();
            foreach (var effect in Effects)
            {
                effects.Add(effect.CreateEffect());
            }
            return new ObjectTargeting(effects, vFXData);
        }
    }
}
