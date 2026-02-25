namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        void TakeControl(IMovementInputReader inputReader);
        void ReleaseControl();
    }
}
