using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ObjectStrategy", menuName = "Targeting/Factory/Object")]
    public class ObjectTargetingFactory : TargetingFactory
    {
        public override TargetingStrategy Create()
        {
            return new ObjectStrategy(vFXData);
        }
    }
}
