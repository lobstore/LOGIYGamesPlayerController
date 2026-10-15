namespace LOGIYGames.AI
{
    public abstract class AIBaseState : IState
    {
        protected AIBrain Brain { get; private set; }

        protected AIBaseState(AIBrain brain)
        {
            Brain = brain;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }
        public virtual bool CanEnter()
        {
            return true;
        }
        public virtual bool CanExit()
        {
            return true;
        }
    }
}
