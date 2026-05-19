using R3;
using UnityEngine;

namespace LOGIYGames
{
    public class InteractorModule : MonoModuleBase
    {
        [Header("Settings")]
        [SerializeField]
        private float interactRadius = 2f;

        [SerializeField]
        private LayerMask interactMask;

        [SerializeField]
        private Transform interactPoint;

        private ReactiveProperty<IInteractable> _currentInteractable = new();

        public ReadOnlyReactiveProperty<IInteractable> CurrentInteractable
            => _currentInteractable;

        public Subject<IInteractable> Interacted = new();
        private void Start()
        {
            //TEST
            CurrentInteractable.Subscribe(x =>
            {
                if (x != null) { Debug.Log(x.GetInteractionData().Name); }
            });
        }
        override public void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            Scan();
        }

        private void Scan()
        {
            Collider[] hits = Physics.OverlapSphere(
                interactPoint.position,
                interactRadius,
                interactMask);

            IInteractable closest = null;

            float closestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IInteractable>(out var interactable))
                {
                    float distance =
                        Vector3.Distance(
                            transform.position,
                            hit.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = interactable;
                    }
                }
            }

            _currentInteractable.Value = closest;
        }

        public void TryInteract()
        {
            if (_currentInteractable.Value == null)
                return;

            _currentInteractable.Value.Interact(gameObject);

            Interacted.OnNext(_currentInteractable.Value);
        }

        private void OnDrawGizmosSelected()
        {
            if (interactPoint == null)
                return;

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                interactPoint.position,
                interactRadius);
        }
    }
}
