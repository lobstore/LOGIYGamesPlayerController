using R3.Triggers;
using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class ObjectAbilityController : MonoBehaviour
    {
        [SerializeField] protected List<Collider> colliders;
        public UnityEvent<Collider> OnProjectileCollided = new();
        public UnityEvent OnProjectileSpawned = new();
        protected AbilityContext context;
        public void Initialize(AbilityContext context, float duration)
        {
            this.context = context;

            Destroy(gameObject, duration);
        }
        protected void Awake()
        {
            OnProjectileSpawned?.Invoke();
            foreach (var item in colliders)
            {
                item.OnTriggerEnterAsObservable().Subscribe(other =>
                {
                    if (!item.enabled)
                    {
                        return;
                    }
                    if (other == context.Source.GetComponent<Collider>())
                        return;
                    OnProjectileCollided?.Invoke(other);
                }
                ).AddTo(this);
            }
        }
    }
}
