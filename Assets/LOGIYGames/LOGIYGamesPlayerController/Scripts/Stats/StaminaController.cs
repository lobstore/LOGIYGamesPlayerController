using LOGIYGames.Timers;
using R3;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames.CharacterCore
{
    public class StaminaController : MonoBehaviour
    {
        public Stamina Stamina { get; private set; }
        CountdownTimer _regenDelayTimer;
        [SerializeField] float delayBeforeRegen;
        Stat VITStat;
        public Stat SPStat { get; private set; }
        public UnityEvent<float> StaminaUsed = new();
        public UnityEvent<float> StaminaRestored = new();
        public UnityEvent Exhausted = new();
        Actor Actor;
        private void Awake()
        {
            Actor = GetComponent<Actor>();
            Stamina = new Stamina();
            _regenDelayTimer = new(delayBeforeRegen);
            StaminaUsed.AddListener((_) =>
            {
                _regenDelayTimer.Start();
            });
        }
        private void Start()
        {
            SPStat = Actor.Stats.GetStat(StatType.BaseStamina);
            VITStat = Actor.Stats.GetStat(StatType.Vitality);
            Stamina.Current = Stamina.Max;
            UpdateMaxValue();
        }
        private void UpdateMaxValue()
        {
            Stamina.Max = SPStat.Value + (VITStat.Value * SPStat.Value * 0.01f);
        }
        private void Update()
        {
            var m_regenAmount = VITStat.Value / 10f + 1;
            if (CanRegenerate())
            {
                Restore(m_regenAmount * Time.deltaTime);

            }
            UpdateMaxValue();
        }
        private bool CanRegenerate()
        {
            return !_regenDelayTimer.IsRunning
                   && Stamina.Current < Stamina.Max;
        }
        public bool CanUse(float amount)
        {
            return Stamina.Current >= amount;
        }

        public bool TryUse(float amount)
        {
            if (!CanUse(amount))
            {
                Exhausted.Invoke();
                return false;
            }

            Stamina.Current -= amount;

            StaminaUsed.Invoke(amount);

            return true;
        }

        public void Restore(float amount)
        {
            float previous = Stamina.Current;

            Stamina.Current = Mathf.Min(Stamina.Current + amount, Stamina.Max);

            float restored = Stamina.Current - previous;

            if (restored > 0)
            {
                StaminaRestored.Invoke(restored);
            }
        }
    }
}
