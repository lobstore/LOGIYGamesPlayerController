public interface IModule
{
    public int ModulePriority { get; }

    public void Initialize();
    public void OnUpdate(float deltaTime);
    public void OnFixedUpdate(float fixedDeltaTime);
    public void OnLateUpdate(float deltaTime);
}