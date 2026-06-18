using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class TargetingManager : MonoBehaviour
    {
        CharacterModule characterModule;

        TargetingStrategy currentStrategy;

        private void Update()
        {
            if (currentStrategy != null && currentStrategy.IsTargeting)
            {
                currentStrategy.Update();
            }
        }

        public void SetStrategy(TargetingStrategy targetingStrategy)
        {
            currentStrategy = targetingStrategy;
        }

        public void ClearStrategy()
        {
            currentStrategy = null;
        }
    }
}
