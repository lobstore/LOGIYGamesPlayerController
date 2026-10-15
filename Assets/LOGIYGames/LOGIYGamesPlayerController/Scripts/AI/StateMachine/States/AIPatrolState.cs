using UnityEngine;

namespace LOGIYGames.AI
{
    public sealed class AIPatrolState : AIBaseState
    {
        private int _currentPointIndex = -1;
        private bool _waiting;
        private float _waitUntil;

        public AIPatrolState(AIBrain brain) : base(brain)
        {
        }

        public override void Enter()
        {
            _waiting = false;

            _currentPointIndex =
                Brain.GetClosestPatrolPointIndex();

            MoveToCurrentPoint();
        }

        public override void Exit()
        {
            _waiting = false;
        }

        public override void LogicUpdate()
        {
            if (!Brain.HasPatrolPoints)
            {
                Brain.StopMoving();
                return;
            }

            if (_waiting)
            {
                if (Time.time < _waitUntil)
                    return;

                _currentPointIndex =
                    Brain.GetNextPatrolPointIndex(
                        _currentPointIndex);

                _waiting = false;

                MoveToCurrentPoint();
                return;
            }

            if (!Brain.HasReachedDestination())
                return;

            Brain.StopMoving();

            _waiting = true;
            _waitUntil = Time.time + Brain.PatrolWaitTime;
        }

        private void MoveToCurrentPoint()
        {
            Transform point =
                Brain.GetPatrolPoint(_currentPointIndex);

            if (point == null)
            {
                Brain.StopMoving();
                return;
            }

            Brain.SetMoveDestination(point.position);
        }
    }
}