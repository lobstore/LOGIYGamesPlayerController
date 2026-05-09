using LOGIYGames.Timers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LOGIYGames
{
    public class NearestInteractableFinder : MonoBehaviour
    {

        [SerializeField] Transform Head;
        [SerializeField] private float _checkInterval = 0.2f; // Интервал проверки в секундах
        [SerializeField] private float _movingTargetCooldown = 1.0f; // Задержка после потери движущейся цели
        [SerializeField] private int maxTrackedObjectNumber;

        [SerializeField] private List<Interactable> _trackablesInRange = new();
        [field: SerializeField] public Interactable CurrentTarget { get; private set; }
        private StopwatchTimer _timer;
        private StopwatchTimer _movingTargetCooldownTimer;
        private bool _wasMovingTargetLastCheck = false;

        private void Awake()
        {
            _timer = new StopwatchTimer();
            _movingTargetCooldownTimer = new StopwatchTimer();
        }

        private void Start()
        {
            _timer.Start();
            _movingTargetCooldownTimer.Start();
        }


        private void Update()
        {
            Tracking();
        }

        private void Tracking()
        {
            if (_timer.CurrentTime >= _checkInterval)
            {
                _timer.Reset();
                UpdateTarget();

            }
        }

        private void UpdateTarget()
        {
            var validTargets = _trackablesInRange
                .Where(t => t != null &&
                           t.gameObject.activeInHierarchy &&
                           t.isActiveAndEnabled)
                .ToList();

            if (validTargets.Count == 0)
            {
                CurrentTarget = null;
                _wasMovingTargetLastCheck = false;
                return;
            }

            // Проверяем движущиеся объекты
            var movingTargets = validTargets.Where(t => t.IsMoving).ToList();
            bool hasMovingTargets = movingTargets.Count > 0;

            // Если есть движущиеся цели - сразу выбираем ближайшую
            if (hasMovingTargets)
            {
                CurrentTarget = GetClosestTarget(movingTargets);
                _wasMovingTargetLastCheck = true;
                _movingTargetCooldownTimer.Reset(); // Сбрасываем таймер задержки
                return;
            }

            // Если в прошлой проверке была движущаяся цель, но сейчас их нет
            if (_wasMovingTargetLastCheck && !hasMovingTargets)
            {
                // Если таймер задержки еще не истек - оставляем текущую цель
                if (_movingTargetCooldownTimer.CurrentTime < _movingTargetCooldown)
                {
                    return;
                }

                // Если таймер истек - переходим к обычной логике выбора
                _wasMovingTargetLastCheck = false;
            }

            // Обычная логика выбора цели по приоритетам
            int minPriority = validTargets.Min(t => t.Priority);
            var candidates = validTargets.Where(t => t.Priority == minPriority).ToList();
            CurrentTarget = GetClosestTarget(candidates);
        }

        private Interactable GetClosestTarget(List<Interactable> targets)
        {
            Interactable closest = null;
            float minDistance = float.MaxValue;

            foreach (var candidate in targets)
            {
                float distance = Vector3.Distance(Head.position, candidate.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = candidate;
                }
            }

            return closest;
        }
        private void OnTriggerEnter(Collider other)
        {
            var trackable = other.GetComponent<Interactable>();
            if (trackable != null && _trackablesInRange.Count < maxTrackedObjectNumber)
            {
                _trackablesInRange.Add(trackable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var trackable = other.GetComponent<Interactable>();
            if (trackable != null)
            {
                _trackablesInRange.Remove(trackable);

                if (CurrentTarget == trackable)
                {
                    // При выходе цели из зоны сразу сбрасываем флаг движущейся цели
                    _wasMovingTargetLastCheck = false;
                    UpdateTarget();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (TryGetComponent<Collider>(out var collider))
            {
                Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}