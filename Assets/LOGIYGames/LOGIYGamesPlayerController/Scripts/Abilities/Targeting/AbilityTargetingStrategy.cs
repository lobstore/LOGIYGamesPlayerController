using System;

[Serializable]
public abstract class AbilityTargetingStrategy
{
    protected Ability ability;
    protected AbilityTargeting targetingManager;
    protected bool isTargeting = false;

    public bool IsTargeting => isTargeting;

    public abstract void Start(Ability ability, AbilityTargeting targetingManager);
    public virtual void Update() { }
    public virtual void Cancel() { }
}
