using LOGIYGames.CharacterCore;
using UnityEngine;

public class AbilityTargetingController : MonoBehaviour {
    public Character Character;
    AbilityTargetingStrategy currentStrategy;

    void Update() {
        if (currentStrategy != null && currentStrategy.IsTargeting) {
            currentStrategy.Update();
        }
    }
    
    public void SetCurrentStrategy(AbilityTargetingStrategy strategy) => currentStrategy = strategy;
    public void ClearCurrentStrategy() => currentStrategy = null;
}