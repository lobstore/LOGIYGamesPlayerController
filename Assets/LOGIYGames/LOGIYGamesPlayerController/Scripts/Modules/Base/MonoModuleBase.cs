using UnityEngine;

public abstract class MonoModuleBase : MonoBehaviour, IModule
{
    
    [field:SerializeField][Tooltip("Less value mean earlier calls")] public int ModulePriority { get; protected set; } = 0;

    public virtual void Initialize() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnFixedUpdate(float fixedDeltaTime) { }
    public virtual void OnLateUpdate(float deltaTime) { }
}
