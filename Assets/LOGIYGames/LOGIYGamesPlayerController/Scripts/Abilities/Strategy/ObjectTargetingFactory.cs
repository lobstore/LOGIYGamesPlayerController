using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ObjectStrategy", menuName = "Targeting/Factory/Object")]
    public class ObjectTargetingFactory : TargetingFactory
    {
        public override AbilityTargetingStrategy Create()
        {
            return new ObjectStrategy(vFXData);
        }
    }
}
