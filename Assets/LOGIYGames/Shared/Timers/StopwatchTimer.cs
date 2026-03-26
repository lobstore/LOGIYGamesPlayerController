using System;
using UnityEngine;
namespace LOGIYGames.Timers
{
    [Serializable]
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        public override void Tick()
        {
            if (IsRunning)
            {
                CurrentTime += Time.deltaTime;
            }
        }
        public override bool IsFinished => IsStopped;
    }
}