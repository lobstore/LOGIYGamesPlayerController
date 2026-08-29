public class SelfTargeting : AbilityTargetingStrategy
{
    public override void Start(Ability ability, AbilityTargetingController targetingManager)
    {
        ability.ApplyEffects(targetingManager.Character);
        ability.CooldownTimer.Start();
    }
}