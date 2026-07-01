namespace LOGIYGames
{
    public class AbilityTargetingController
    {
        AbilityTargetingStrategy currentStrategy;

        public void Tick()
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
