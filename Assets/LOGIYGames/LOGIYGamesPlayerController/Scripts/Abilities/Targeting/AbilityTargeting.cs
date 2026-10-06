using LOGIYGames.CharacterCore;

public class AbilityTargeting
{
    public Actor Character { get; private set; }
    AbilityTargetingStrategy currentStrategy;
    public bool IsTargeting => currentStrategy != null && currentStrategy.IsTargeting;
    public AbilityTargeting(Actor actor)
    {
        Character = actor;
    }
    public void Update()
    {
        if (currentStrategy != null && currentStrategy.IsTargeting)
        {
            currentStrategy.Update();
        }
    }

    public void SetCurrentStrategy(AbilityTargetingStrategy strategy) => currentStrategy = strategy;
    public void ClearCurrentStrategy() => currentStrategy = null;
}