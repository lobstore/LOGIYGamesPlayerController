using R3;
using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class ProjectileAbilityController : MonoBehaviour
    {
        [SerializeField] List<Collider> colliders;
        public UnityEvent<Collider> OnProjectileCollided = new();
        public UnityEvent OnProjectileSpawned = new();
        AbilityContext context;
        private float speed;
        [field: SerializeField] public bool IsStopped { get; private set; }
        public void Initialize(AbilityContext context, float speed)
        {
            this.context = context;
            this.speed = speed;

            Destroy(gameObject, 5f);
        }
        private void Update()
        {

            
            Move();

        }
        private void Awake()
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
        private void Move()
        {
            if (IsStopped) return;
            if (context.Target != null)
            {
                gameObject.transform.Translate(gameObject.transform.position - context.Source.transform.position * Time.deltaTime * speed);

            }
            else
            {

                gameObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            }
        }
        public void StopMove()
        {
            IsStopped = true;
        }
    }
}
