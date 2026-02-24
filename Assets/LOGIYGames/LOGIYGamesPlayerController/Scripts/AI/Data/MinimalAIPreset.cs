using LOGIYGames.AI;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MinimalAIPreset", menuName = "AIStateMachine/AIStatesPreset/MinimalAI")]
    public class MinimalAIPreset : AIStatesPresetBase
    {
        // AI States
        // TODO Make Builder for AI Archetypes and AI configuration
        private AIIdleState _idleState;
        private AIPatrolState _patrolState;
        private AIChaseState _chaseState;
        private AIAttackState _attackState;
        public override void Init(AIBrainStateDriver AIBrainStateDriver)
        {
            // Initialize all states
            _idleState = new AIIdleState(AIBrainStateDriver, AIBrainStateDriver.MinIdleDuration, AIBrainStateDriver.MaxIdleDuration);
            _patrolState = new AIPatrolState(AIBrainStateDriver, AIBrainStateDriver.PatrolArrivalThreshold);
            _chaseState = new AIChaseState(AIBrainStateDriver, AIBrainStateDriver.LostTargetTimeout);
            _attackState = new AIAttackState(AIBrainStateDriver);
            ConfigureTransitions(AIBrainStateDriver);

            AIBrainStateDriver.StateMachine.SetState(_idleState);
        }
        /// <summary>
        /// Configures all AI behavior state transitions
        /// </summary>
        private void ConfigureTransitions(AIBrainStateDriver AIBrainStateDriver)
        {
            // ----- Idle State Transitions -----
            AIBrainStateDriver.AddTransition(_idleState, _patrolState, () =>
                AIBrainStateDriver.PatrolPoints != null && AIBrainStateDriver.PatrolPoints.Length > 0 && _idleState.IsIdleComplete());
            AIBrainStateDriver.AddTransition(_idleState, _chaseState, () =>
                AIBrainStateDriver.Target != null && AIBrainStateDriver.IsTargetDetected());

            // ----- Patrol State Transitions -----
            AIBrainStateDriver.AddTransition(_patrolState, _idleState, () =>
                _patrolState.HasReachedPatrolPoint());
            AIBrainStateDriver.AddTransition(_patrolState, _chaseState, () =>
                AIBrainStateDriver.Target != null && AIBrainStateDriver.IsTargetDetected());

            // ----- Chase State Transitions -----
            AIBrainStateDriver.AddTransition(_chaseState, _attackState, () =>
                AIBrainStateDriver.Target != null && _attackState.IsTargetInAttackRange());
            AIBrainStateDriver.AddTransition(_chaseState, _patrolState, () =>
                AIBrainStateDriver.HasLostTarget() && AIBrainStateDriver.PatrolPoints != null && AIBrainStateDriver.PatrolPoints.Length > 0);
            AIBrainStateDriver.AddTransition(_chaseState, _idleState, () =>
                AIBrainStateDriver.HasLostTarget() && (AIBrainStateDriver.PatrolPoints == null || AIBrainStateDriver.PatrolPoints.Length == 0));

            // ----- Attack State Transitions -----
            AIBrainStateDriver.AddTransition(_attackState, _chaseState, () =>
                AIBrainStateDriver.Target != null && !_attackState.IsTargetInAttackRange());
            AIBrainStateDriver.AddTransition(_attackState, _patrolState, () =>
                AIBrainStateDriver.HasLostTarget() && AIBrainStateDriver.PatrolPoints != null && AIBrainStateDriver.PatrolPoints.Length > 0);
            AIBrainStateDriver.AddTransition(_attackState, _idleState, () =>
                AIBrainStateDriver.HasLostTarget() && (AIBrainStateDriver.PatrolPoints == null || AIBrainStateDriver.PatrolPoints.Length == 0));
        }
    }
}
