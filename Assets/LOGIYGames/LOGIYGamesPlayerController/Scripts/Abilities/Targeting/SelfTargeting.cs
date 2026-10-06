public class SelfTargeting : AbilityTargetingStrategy
{
    public override void Start(Ability ability, AbilityTargeting targetingManager)
    {
        ability.ApplyEffects(targetingManager.Character);
        ability.CooldownTimer.Start();
    }
}