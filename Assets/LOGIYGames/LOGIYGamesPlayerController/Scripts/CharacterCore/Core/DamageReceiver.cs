namespace LOGIYGames.CharacterCore
{
    public class DamageReceiver
    {
        Health Health;

        public DamageReceiver(Health health)
        {
            Health = health;
        }

        public void TakeDamage(in DamageContext damage)
        {
            if (!damage.Cancelled)
            {
                Health.ApplyDamage(damage.Damage);
            }
        }
    }
}
