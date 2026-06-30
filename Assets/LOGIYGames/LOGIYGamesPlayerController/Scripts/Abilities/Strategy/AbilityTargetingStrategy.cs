namespace LOGIYGames
{
    public abstract class AbilityTargetingStrategy
    {
        protected Ability Ability;
        protected AbilityController AbilityController;
        public bool IsTargeting {  get; private set; }
        public abstract void Start(Ability ability, AbilityController abilityController);
        public virtual void Update() { }
        public virtual void Cancel() { }
    }
}
