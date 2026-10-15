namespace LOGIYGames.AI
{
    public sealed class AIIdleState : AIBaseState
    {
        public AIIdleState(AIBrain brain) : base(brain)
        {
        }

        public override void Enter()
        {
            Brain.StopMoving();
        }

        public override void Exit()
        {
        }

        public override void LogicUpdate()
        {
            // Переходы выполняет StateMachine.
        }
    }
}