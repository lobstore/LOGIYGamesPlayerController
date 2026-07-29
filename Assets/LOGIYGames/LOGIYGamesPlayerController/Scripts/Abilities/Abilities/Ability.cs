using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
public class Ability
{
    public CountdownTimer CooldownTimer { get; private set; }
    public AbilitySO Data { get; private set; }
    public Ability(AbilitySO abilitySO)
    {
        Data = abilitySO;
        CooldownTimer = new(abilitySO.Cooldown);
    }
    public virtual void Target(AbilityTargetingController targetingManager)
    {
        if (Data.targetingStrategy != null)
        {
            Data.targetingStrategy.Start(this, targetingManager);
        }
    }

    public virtual void Execute(Character target)
    {
        foreach (var effect in Data.effects)
        {
            var runtimeEffect = effect.Create();
            target?.EffectSystem.AddEffect(runtimeEffect);
        }
    }
}