using Unity.Netcode;

namespace LOGIYGames
{
    public abstract class NetworkModuleBase : NetworkBehaviour, IModule
    {
        public int ModulePriority { get; protected set; } = 0;

        public virtual void Initialize()
        {
            if (!IsOwner) return;
        }
        public virtual void OnUpdate(float deltaTime)
        {
            if (!IsOwner) return;
        }
        public virtual void OnFixedUpdate(float fixedDeltaTime)
        {
            if (!IsOwner) return;
        }
        public virtual void OnLateUpdate(float deltaTime)
        {
            if (!IsOwner) return;
        }
    }
}
