using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;

public class Ability
{
    public CountdownTimer CooldownTimer { get; private set; }
    public AbilitySO Data {  get; private set; }
    public Ability(AbilitySO abilitySO)
    {
        this.Data = abilitySO;
        CooldownTimer = new(abilitySO.Cooldown);
    }
    public void Target(AbilityTargetingController targetingManager)
    {
        if (Data.targetingStrategy != null)
        {
            Data.targetingStrategy.Start(this, targetingManager);
        }
    }

    public void Execute(Character target)
    {

        CooldownTimer.Start();
        foreach (var effect in Data.effects)
        {
            var runtimeEffect = effect.Create();
            target?.EffectSystem.AddEffect(runtimeEffect);
        }
    }
}
public class PassiveAbility : Ability
{
    public PassiveAbility(AbilitySO abilitySO) : base(abilitySO)
    {
    }
    public bool Evaluate()
    {
        return true;
    }
}