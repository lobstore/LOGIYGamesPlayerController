namespace LOGIYGames
{
    /// <summary>
    /// Base interface for all states
    /// </summary>
    public interface IState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
        void LateUpdate();
        void PhysicsUpdate();
    }

}
