using System;

namespace LOGIYGames.CharacterCore
{
    public class DamageController
    {
        Health Health;
        Stat VITStat;
        public DamageController(Health health, CharacterStats stats)
        {
            VITStat = stats.GetStat(StatType.Vitality);
            Health = health;
        }

        public void TakeDamage(in DamageContext damage)
        {
            if (!damage.Cancelled)
            {
                float resaultDamage = Math.Clamp(damage.Damage - VITStat.Value,0, float.MaxValue );
                Health.ApplyDamage(resaultDamage);
            }
        }
    }
}
