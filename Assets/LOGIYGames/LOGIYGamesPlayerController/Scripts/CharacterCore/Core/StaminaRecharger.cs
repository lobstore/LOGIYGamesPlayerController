using LOGIYGames.Timers;
using R3;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class StaminaRecharger
    {
        Stamina Stamina;
        float m_regenAmount;
        CountdownTimer _regenDelayTimer;
        public StaminaRecharger(Stamina stamina, float delayBeforeRegen = 0, float regenAmount = 5f)
        {
            Stamina = stamina;
            m_regenAmount = regenAmount;
            _regenDelayTimer = new(delayBeforeRegen);
            Stamina.StaminaUsed.Subscribe((_) =>
            {
                _regenDelayTimer.Start();
            });
        }

        public void Tick()
        {
            if (CanRegenerate())
            {
                Stamina.Restore(m_regenAmount * Time.deltaTime);

            }
        }
        private bool CanRegenerate()
        {
            return !_regenDelayTimer.IsRunning
                   && Stamina.Current.CurrentValue< Stamina.Max.CurrentValue;
        }
    }
}
