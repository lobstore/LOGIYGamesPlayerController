using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ProjectileStrategy", menuName = "Targeting/Factory/Projectile")]
    public class ProjectileTargetingFactory : TargetingFactory
    {
        public float speed;
        public override TargetingStrategy Create()
        {
            var effects = new List<IEffect>();
            foreach (var effect in Effects)
            {
                effects.Add(effect.CreateEffect());
            }
            return new ProjectileTargeting(effects, vFXData, speed);
        }
    }
}
