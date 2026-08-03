using LOGIYGames.CharacterCore;
using UnityEngine;

public class AbilityTargetingController : MonoBehaviour {
    public Character Character {  get; private set; }
    AbilityTargetingStrategy currentStrategy;
    private void Awake()
    {
        if (Character == null)
        {
            Character = GetComponent<Character>();
        }
    }
    void Update() {
        if (currentStrategy != null && currentStrategy.IsTargeting) {
            currentStrategy.Update();
        }
    }
    
    public void SetCurrentStrategy(AbilityTargetingStrategy strategy) => currentStrategy = strategy;
    public void ClearCurrentStrategy() => currentStrategy = null;
}