using LOGIYGames.CharacterCore;
using UnityEngine;

public class AbilityTargetingController : MonoBehaviour
{
    public Actor Character { get; private set; }
    AbilityTargetingStrategy currentStrategy;
    public bool IsTargeting => currentStrategy != null && currentStrategy.IsTargeting;
    private void Awake()
    {
        if (Character == null)
        {
            Character = GetComponent<Actor>();
        }
    }
    void Update()
    {
        if (currentStrategy != null && currentStrategy.IsTargeting)
        {
            currentStrategy.Update();
        }
    }

    public void SetCurrentStrategy(AbilityTargetingStrategy strategy) => currentStrategy = strategy;
    public void ClearCurrentStrategy() => currentStrategy = null;
}