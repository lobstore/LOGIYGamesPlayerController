namespace LOGIYGames
{

    public interface IEffect
    {
        void Apply(AbilityContext context);

        void Cancel() { }
    }
}
