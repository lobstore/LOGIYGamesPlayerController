using LOGIYGames.Shared.Enums;
using LOGIYGames.Timers;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboInputBuffer
    {
        private AttackInputType
            _bufferedInput;

        private readonly CountdownTimer
            _bufferTimer;

        public ComboInputBuffer(
            float bufferDuration = 0.25f)
        {
            _bufferTimer =
                new CountdownTimer(
                    bufferDuration);

            _bufferTimer.OnTimerStop +=
                Clear;
        }

        // =====================================================
        // BUFFER
        // =====================================================

        public void BufferInput(
            AttackInputType input)
        {
            _bufferedInput = input;

            _bufferTimer.Reset();

            _bufferTimer.Start();
        }

        // =====================================================
        // HELPERS
        // =====================================================

        public bool HasInput()
        {
            return _bufferedInput
                   != AttackInputType.None;
        }

        public AttackInputType ConsumeInput()
        {
            AttackInputType input =
                _bufferedInput;

            Clear();

            return input;
        }

        // =====================================================
        // CLEAR
        // =====================================================

        public void Clear()
        {
            _bufferedInput =
                AttackInputType.None;
        }
    }
}
