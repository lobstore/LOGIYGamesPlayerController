using LOGIYGames.CharacterCore;
using System;

[Serializable]
public class SelfTargeting : AbilityTargetingStrategy
{
    public override void Start(Ability ability, AbilityTargetingController targetingManager)
    {
        ability.Execute(targetingManager.Character);
        ability.CooldownTimer.Start();
    }
}