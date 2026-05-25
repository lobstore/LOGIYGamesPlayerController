using System;

namespace LOGIYGames
{
    public interface IPredicate
    {
        bool Evaluate();
    }
    public interface ITransition
    {
        Type To { get; }
        IPredicate Condition { get; }
    }
    public class Transition : ITransition
    {
        public Type To { get; }
        public IPredicate Condition { get; }

        public Transition(Type to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}