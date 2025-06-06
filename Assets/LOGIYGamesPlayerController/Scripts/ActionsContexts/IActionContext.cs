namespace LOGIYGames
{
    public interface IActionContext
    {
        public MotionType MotionType { get; }
        public void EnterState();
        public void ExitState();
        public void OnUpdate();
    }
    public enum MotionType
    {
        RigidBody,
        CharacterController,
        AnimatorController
    }
}