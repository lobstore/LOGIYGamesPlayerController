using UnityEngine;

namespace LOGIYGames
{
    public class AbilityTargetingController : MonoBehaviour
    {
        AbilityTargetingStrategy currentStrategy;

        private void Update()
        {
            if (currentStrategy != null && currentStrategy.IsTargeting)
            {
                currentStrategy.Update();
            }
        }

        public void SetStrategy(AbilityTargetingStrategy targetingStrategy)
        {
            currentStrategy = targetingStrategy;
        }

        public void ClearStrategy()
        {
            currentStrategy = null;
        }
    }
}
