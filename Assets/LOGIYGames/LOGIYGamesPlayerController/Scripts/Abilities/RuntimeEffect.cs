using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;

namespace LOGIYGames
{
    public abstract class RuntimeEffect
    {
        public Character Owner;
        public CountdownTimer Timer {  get; protected set; }
        public virtual void OnApply()
        {
            Timer.Start();
        }
        public virtual void OnRemove()
        {
            Timer.Stop();
        }
        public abstract void OnUpdate(float delta);
    }

}
