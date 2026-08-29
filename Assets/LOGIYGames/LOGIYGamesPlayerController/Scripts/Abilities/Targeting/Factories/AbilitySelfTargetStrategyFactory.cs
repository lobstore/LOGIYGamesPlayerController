using System;

[Serializable]
public class AbilitySelfTargetStrategyFactory : AbilityTargetingStrategyFactory
{
    public override AbilityTargetingStrategy Create()
    {
        return new SelfTargeting();
    }
}
