using UnityEngine;
using UnityEngine.AI;

namespace LOGIYGames.AI
{
    public sealed class AICombatState : AIBaseState
    {
        private const float ComboAttackInterval = 0.2f;

        private float _nextAttackTime;
        private float _attackLockUntil;
        private float _nextRepathTime;
        private float _nextOrbitChange;
        private float _nextComboAttackTime;

        private Vector3 _lastTargetPosition;
        private int _orbitDirection = 1;

        private int _comboAttacksRemaining;
        private bool _isComboActive;

        public AICombatState(AIBrain brain) : base(brain)
        {
        }

        public override void Enter()
        {
            _attackLockUntil = 0f;
            _nextRepathTime = 0f;
            _isComboActive = false;
            _comboAttacksRemaining = 0;

            _nextAttackTime = Time.time +
                              Random.Range(0.25f, 0.65f);

            _orbitDirection = Random.value < 0.5f ? -1 : 1;
            _nextOrbitChange = Time.time +
                               Random.Range(1.2f, 2.4f);

            UpdateCombatDestination();
        }

        public override void Exit()
        {
            _isComboActive = false;
            _comboAttacksRemaining = 0;

            Brain.StopMoving();
        }

        public override void LogicUpdate()
        {
            Transform target = Brain.Target;

            if (target == null)
            {
                _isComboActive = false;
                Brain.StopMoving();
                return;
            }

            // Выполняем текущую комбинацию до её завершения.
            if (_isComboActive)
            {
                UpdateCombo(target);
                Brain.StopMoving();
                return;
            }

            // Небольшая пауза после серии атак.
            if (Time.time < _attackLockUntil)
            {
                Brain.StopMoving();
                return;
            }

            if (TryStartCombo(target))
                return;

            if (Time.time >= _nextOrbitChange)
            {
                _orbitDirection = Random.value < 0.5f ? -1 : 1;

                _nextOrbitChange = Time.time +
                                   Random.Range(1.2f, 2.4f);
            }

            float targetMovedDistance =
                (target.position - _lastTargetPosition).sqrMagnitude;

            bool targetMoved = targetMovedDistance >= 0.36f;

            NavMeshAgent agent = Brain.Agent;

            bool agentStopped =
                agent == null || agent.isStopped;

            if (Time.time >= _nextRepathTime ||
                targetMoved ||
                agentStopped)
            {
                UpdateCombatDestination();
            }
        }

        private bool TryStartCombo(Transform target)
        {
            if (Time.time < _nextAttackTime)
                return false;

            if (!Brain.IsTargetInAttackRange())
                return false;

            Vector3 direction =
                target.position - Brain.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return false;

            float angle = Vector3.Angle(
                Brain.transform.forward,
                direction);

            if (angle > Brain.AttackAngle)
                return false;

            // Случайное количество атак: 1, 2 или 3.
            _comboAttacksRemaining = Random.Range(1, 4);
            _isComboActive = true;

            _nextAttackTime = Time.time +
                              Mathf.Max(0.1f, Brain.AttackCooldown);

            // ИИ не начинает двигаться, пока не завершит серию
            // и минимальное время фиксации атаки.
            float comboDuration =
                (_comboAttacksRemaining - 1) * ComboAttackInterval;

            _attackLockUntil = Time.time +
                Mathf.Max(Brain.AttackCommitTime, comboDuration);

            // Первая атака выполняется немедленно.
            PerformComboAttack(target);

            return true;
        }

        private void UpdateCombo(Transform target)
        {
            if (Time.time < _nextComboAttackTime)
                return;

            PerformComboAttack(target);
        }

        private void PerformComboAttack(Transform target)
        {
            if (target == null || _comboAttacksRemaining <= 0)
            {
                _comboAttacksRemaining = 0;
                _isComboActive = false;
                return;
            }

            Brain.RequestAttack(target);

            _comboAttacksRemaining--;

            if (_comboAttacksRemaining <= 0)
            {
                _isComboActive = false;
                return;
            }

            // Следующая атака через 0,1 секунды.
            _nextComboAttackTime =
                Time.time + ComboAttackInterval;
        }

        private void UpdateCombatDestination()
        {
            Transform target = Brain.Target;
            NavMeshAgent agent = Brain.Agent;

            if (target == null ||
                agent == null ||
                !agent.isOnNavMesh)
            {
                return;
            }

            Vector3 toTarget =
                target.position - Brain.transform.position;

            toTarget.y = 0f;

            float distance = toTarget.magnitude;

            if (distance < 0.001f)
                return;

            Vector3 awayDirection = -toTarget.normalized;

            float preferredDistance = Mathf.Max(
                Brain.PreferredCombatDistance,
                Brain.AttackRange * 0.8f);

            Vector3 destination;

            if (distance > preferredDistance + 0.45f)
            {
                // Сближение с противником.
                destination = target.position +
                              awayDirection * preferredDistance;
            }
            else if (distance < Brain.MinCombatDistance)
            {
                // Отступление при чрезмерном сближении.
                Vector3 retreatDirection = Quaternion.Euler(
                    0f,
                    _orbitDirection * 20f,
                    0f) * awayDirection;

                destination = target.position +
                              retreatDirection * preferredDistance;
            }
            else
            {
                // Обход противника по дуге.
                Vector3 orbitDirection = Quaternion.Euler(
                    0f,
                    _orbitDirection * 50f,
                    0f) * awayDirection;

                destination = target.position +
                              orbitDirection * preferredDistance;
            }

            float sampleRadius = Mathf.Max(1f, agent.radius * 3f);

            if (!NavMesh.SamplePosition(
                    destination,
                    out NavMeshHit hit,
                    sampleRadius,
                    agent.areaMask))
            {
                _nextRepathTime = Time.time + 0.25f;
                return;
            }

            Brain.SetMoveDestination(hit.position);

            _lastTargetPosition = target.position;

            _nextRepathTime = Time.time +
                              Brain.CombatRepathInterval;
        }
    }
}