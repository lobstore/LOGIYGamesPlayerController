namespace LOGIYGames
{
    public interface IState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
        void LateUpdate();
        void PhysicsUpdate();
    }

}
