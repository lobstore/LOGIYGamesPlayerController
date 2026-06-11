using R3;
using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class ProjectileAbilityController : ObjectAbilityController
    {
        private float speed;
        [field: SerializeField] public bool IsStopped { get; private set; }
        public void Initialize(AbilityContext context, float duration, float speed)
        {
            this.context = context;
            this.speed = speed;

            Destroy(gameObject, duration);
        }
        private void Update()
        {
            Move();
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
