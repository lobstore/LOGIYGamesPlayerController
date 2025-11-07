using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames
{
    public class StaminaController : MonoBehaviour
    {
        CountdownTimer rechargeCooldownTimer;
        [SerializeField] float cooldownTime = 1f;
        StaminaModel StaminaModel;
        [SerializeField] private float rechargeAmount = 1;

        void Start()
        {
            if (StaminaModel == null)
            {
                StaminaModel = GetComponent<StaminaModel>();
            }
            rechargeCooldownTimer = new CountdownTimer(cooldownTime);
            rechargeCooldownTimer.Start();
            StaminaModel.CurrentValueChanged.AddListener((t) =>
            {
                if (t < StaminaModel.CurrentValue)
                {
                    rechargeCooldownTimer.Start();
                }
            });
        }
        void Update()
        {
            if (!rechargeCooldownTimer.IsRunning && StaminaModel.CurrentValue < StaminaModel.MaxValue)
            {
                StaminaModel.CurrentValue += Time.deltaTime * rechargeAmount;
            }
        }
    }
}
