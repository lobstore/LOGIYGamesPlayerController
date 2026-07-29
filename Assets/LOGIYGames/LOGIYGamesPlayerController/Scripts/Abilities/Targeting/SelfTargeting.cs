using LOGIYGames.CharacterCore;
using System;

[Serializable]
public class SelfTargeting : AbilityTargetingStrategy
{
    public override void Start(Ability ability, AbilityTargetingController targetingManager)
    {
        ability.Execute(targetingManager.GetComponent<Character>());
        ability.CooldownTimer.Start();
    }
}