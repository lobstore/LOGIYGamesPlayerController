using System;
using UnityEngine;
namespace LOGIYGames.Timers
{
    [Serializable]
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }
        public override float ElapsedTime => initialTime - CurrentTime.CurrentValue;
        public override void Tick()
        {
            if (IsRunning && CurrentTime.CurrentValue > 0)
            {
                CurrentTime.Value -= Time.deltaTime;
            }

            if (IsRunning && CurrentTime.CurrentValue <= 0)
            {
                Stop();
            }
        }
        public override bool IsFinished => CurrentTime.CurrentValue <= 0;

    }
}