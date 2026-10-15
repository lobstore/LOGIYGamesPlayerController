using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponData Data;
        [SerializeField] Collider Collider;
        GameObject Owner;
        private HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
        public void EnableCollider()
        {
            Collider.enabled = true;
            damagedTargets.Clear();
            Debug.Log("EnableCollider");
        }
        public void DisableCollider()
        {
            Collider.enabled = false;
            damagedTargets.Clear();
            Debug.Log("DisableCollider");
        }
        private void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null && other.gameObject != Owner)
            {
                if (!damagedTargets.Add(damageable))
                    return;
                damageable.TakeDamage(new DamageData
                {
                    Amount = Data.BaseDamage.Amount,
                    From = Owner.GetComponent<IDamageable>(),
                    ModifierType = Data.BaseDamage.ModifierType,
                    Type = Data.BaseDamage.Type

                });
            }
        }

    }
}
