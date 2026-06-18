using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ProjectileStrategy", menuName = "Targeting/Factory/Projectile")]
    public class ProjectileTargetingFactory : TargetingFactory
    {
        public float speed;
        public override TargetingStrategy Create()
        {
            return new ProjectileTargeting(vFXData, speed);
        }
    }
}
