namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        void TakeControl(IInputReader inputReader);
        void ReleaseControl();
    }
}
