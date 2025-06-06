using UnityEngine;

public abstract class MonoModuleBase : MonoBehaviour, IModule
{
    public int ModulePriority { get; protected set; } = 0;

    public virtual void Initialize() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnFixedUpdate(float fixedDeltaTime) { }
    public virtual void OnLateUpdate(float deltaTime) { }
}
