using System;
using UnityEngine;
namespace LOGIYGames.Timers
{
    [Serializable]
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }
        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }

            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }
        public override float Progress => 1-base.Progress;
        public override bool IsFinished => CurrentTime <= 0;

    }
}