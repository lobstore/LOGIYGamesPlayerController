using LOGIYGames.Shared.Enums;
using LOGIYGames.Timers;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboController
    {
        public AttackNodeSO CurrentAttack
        {
            get;
            private set;
        }

        private readonly Character
            _character;

        private readonly Animator
            _animator;

        private CountdownTimer
            _attackTimer;

        private AttackNodeSO
            _queuedAttack;

        private bool _finished;

        public ComboController(
            Character character)
        {
            _character = character;

            _animator =
                character.GetComponent<Animator>();
        }

        // =====================================================
        // HELPERS
        // =====================================================

        private float ElapsedTime =>
            CurrentAttack.Duration
            - _attackTimer.CurrentTime;

        // =====================================================
        // START COMBO
        // =====================================================

        public void StartCombo(
            AttackNodeSO attack)
        {
            if (attack == null)
                return;

            _finished = false;

            _queuedAttack = null;

            PlayAttack(attack);
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public void Tick()
        {
            if (CurrentAttack == null)
                return;

            ReadBufferedInput();

            TryTransition();

            TryFinish();
        }


        // =====================================================
        // INPUT
        // =====================================================

        private void ReadBufferedInput()
        {
            if (!_character.ComboBuffer
                    .HasInput())
                return;

            if (!CanQueueInput())
                return;

            AttackInputType input =
                _character.ComboBuffer
                    .ConsumeInput();

            foreach (AttackTransition
                     transition
                     in CurrentAttack.Transitions)
            {
                if (transition.Input != input)
                    continue;

                _queuedAttack =
                    transition.NextAttack;

                return;
            }
        }

        // =====================================================
        // TRANSITION
        // =====================================================

        private void TryTransition()
        {
            if (_queuedAttack == null)
                return;

            if (!CanTransition())
                return;

            PlayAttack(_queuedAttack);

            _queuedAttack = null;
        }

        // =====================================================
        // PLAY ATTACK
        // =====================================================

        private void PlayAttack(
            AttackNodeSO attack)
        {
            DisposeCurrentTimer();

            CurrentAttack = attack;

            _attackTimer =
                new CountdownTimer(
                    attack.Duration);

            _attackTimer.Start();

            _animator.applyRootMotion =
                attack.UseRootMotion;

            _animator.CrossFade(
                attack.AnimationStateName,
                attack.CrossFade);

            ApplyImpulse(attack);
        }

        // =====================================================
        // IMPULSE
        // =====================================================

        private void ApplyImpulse(
            AttackNodeSO attack)
        {
            if (Mathf.Approximately(
                    attack.ForwardImpulse,
                    0))
                return;

            Vector3 impulse =
                _character.transform.forward
                * attack.ForwardImpulse;

            _character.VelocityData
                .Locomotion += impulse;
        }

        // =====================================================
        // WINDOWS
        // =====================================================

        private bool CanQueueInput()
        {
            return ElapsedTime
                   >= CurrentAttack
                       .ComboWindowStart
                   &&
                   ElapsedTime
                   <= CurrentAttack
                       .ComboWindowEnd;
        }

        private bool CanTransition()
        {
            return ElapsedTime
                   >= CurrentAttack
                       .TransitionTime;
        }

        public bool CanCancel()
        {
            if (CurrentAttack == null)
                return false;

            return ElapsedTime
                   >= CurrentAttack
                       .CancelTime;
        }

        // =====================================================
        // FINISH
        // =====================================================

        private void TryFinish()
        {
            if (_queuedAttack != null)
                return;

            if (!_attackTimer.IsFinished)
                return;

            _finished = true;
        }

        public bool IsFinished()
        {
            return _finished;
        }

        // =====================================================
        // TIMER CONTROL
        // =====================================================

        public void Pause()
        {
            _attackTimer?.Pause();
        }

        public void Resume()
        {
            _attackTimer?.Resume();
        }

        // =====================================================
        // STOP
        // =====================================================

        public void Stop()
        {
            DisposeCurrentTimer();

            CurrentAttack = null;

            _queuedAttack = null;

            _finished = false;

        }

        private void DisposeCurrentTimer()
        {
            if (_attackTimer == null)
                return;

            _attackTimer.Stop();

            _attackTimer.Dispose();

            _attackTimer = null;
        }
    }
}
