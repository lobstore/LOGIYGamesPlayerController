using System;

[Serializable]
public abstract class AbilityTargetingStrategy
{
    protected Ability ability;
    protected AbilityTargetingController targetingManager;
    protected bool isTargeting = false;

    public bool IsTargeting => isTargeting;

    public abstract void Start(Ability ability, AbilityTargetingController targetingManager);
    public virtual void Update() { }
    public virtual void Cancel() { }
}
