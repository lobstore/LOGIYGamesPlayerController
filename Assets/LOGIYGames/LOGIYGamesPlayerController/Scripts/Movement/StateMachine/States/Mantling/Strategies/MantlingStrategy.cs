using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;
using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class MantlingStrategy
    {
        public Transform MantleTargetTransform { get; protected set; }
        public Vector3 MantleTargetLocalPoint { get; protected set; }
        public Vector3 MantleTargetPosition { get; protected set; }
        public Vector3 MantleStartPosition { get; protected set; }

        public RaycastHit TargetTopPoint { get; protected set; }

        protected Character _characterModule;
        protected float checkDistance;
        protected LayerMask mantlingLayers;
        public CountdownTimer Duration { get; protected set; }
        public MantlingType MantlingType { get; protected set; }
        protected MantlingStrategy(Character chr, MantlingData data)
        {
            this.mantlingLayers = data.mantlingLayers;
            this.checkDistance = data.checkDistance;
            _characterModule = chr;
            Duration = new CountdownTimer(data.duration);
        }

        abstract public bool CanEnter();
        abstract public bool CanExit();
        abstract public void Enter();
        abstract public void Exit();
        abstract public void Tick();
    }
}
