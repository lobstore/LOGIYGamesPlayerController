using LOGIYGames.AI;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MinimalAIPreset", menuName = "AIStateMachine/AIStatesPreset/MinimalAI")]
    public class MinimalAIPreset : AIStatesPresetBase
    {
        public override void Init(AIBrain AIBrainStateDriver)
        {

            //_chaseState = new AIChaseState(AIBrainStateDriver, AIBrainStateDriver.LostTargetTimeout);
            //_attackState = new AIAttackState(AIBrainStateDriver);

            AIBrainStateDriver.StateMachine.AddState(new AIIdleState(AIBrainStateDriver, AIBrainStateDriver.MinIdleDuration, AIBrainStateDriver.MaxIdleDuration));
            AIBrainStateDriver.StateMachine.AddState(new AIPatrolState(AIBrainStateDriver, AIBrainStateDriver.PatrolArrivalThreshold));
            //AIBrainStateDriver.StateMachine.AddState(_chaseState);
           //AIBrainStateDriver.StateMachine.AddState(_attackState);

            ConfigureTransitions(AIBrainStateDriver);

            AIBrainStateDriver.StateMachine.SetState<AIIdleState>();
        }
        /// <summary>
        /// Configures all AI behavior state transitions
        /// </summary>
        private void ConfigureTransitions(AIBrain AIBrainStateDriver)
        {
            // ----- Idle State Transitions -----
            AIBrainStateDriver.StateMachine.AddTransition<AIIdleState, AIPatrolState>(
                new FuncPredicate(() =>
            {
                return AIBrainStateDriver.PatrolPoints != null && AIBrainStateDriver.PatrolPoints.Length > 0 && AIBrainStateDriver.StateMachine.GetState<AIIdleState>().IsIdleComplete();
            }));
            AIBrainStateDriver.StateMachine.AddTransition<AIIdleState, AIChaseState>(
             new FuncPredicate(() =>
            {
                return AIBrainStateDriver.TargetingController.CurrentTarget != null && AIBrainStateDriver.IsTargetDetected();
            })
             );


            // ----- Patrol State Transitions -----
            AIBrainStateDriver.StateMachine.AddTransition<AIPatrolState, AIIdleState>(
                new FuncPredicate(() =>
            {
                return AIBrainStateDriver.StateMachine.GetState<AIPatrolState>().HasReachedPatrolPoint();
            }));

            AIBrainStateDriver.StateMachine.AddTransition<AIPatrolState, AIChaseState>(
                new FuncPredicate(() =>
                {
                    return AIBrainStateDriver.TargetingController.CurrentTarget != null && AIBrainStateDriver.IsTargetDetected();
                }));


            // ----- Chase State Transitions -----
            AIBrainStateDriver.StateMachine.AddTransition<AIChaseState, AIAttackState>(
             new FuncPredicate(() =>
             {
                 return AIBrainStateDriver.TargetingController.CurrentTarget != null && AIBrainStateDriver.StateMachine.GetState<AIAttackState>().IsTargetInAttackRange();
             }));

            AIBrainStateDriver.StateMachine.AddTransition<AIChaseState, AIPatrolState>(
            new FuncPredicate(() =>
            {
                return AIBrainStateDriver.HasLostTarget() && AIBrainStateDriver.PatrolPoints != null && AIBrainStateDriver.PatrolPoints.Length > 0;
            }));

            AIBrainStateDriver.StateMachine.AddTransition<AIChaseState, AIIdleState>(
                                                                new FuncPredicate(() =>
            {
                return AIBrainStateDriver.HasLostTarget() && (AIBrainStateDriver.PatrolPoints == null || AIBrainStateDriver.PatrolPoints.Length == 0);
            }));


            // ----- Attack State Transitions -----
            AIBrainStateDriver.StateMachine.AddTransition<AIAttackState, AIChaseState>(
            new FuncPredicate(() =>
            {
                return AIBrainStateDriver.TargetingController.CurrentTarget != null && !AIBrainStateDriver.StateMachine.GetState<AIAttackState>().IsTargetInAttackRange();
            }));

            AIBrainStateDriver.StateMachine.AddTransition<AIAttackState, AIPatrolState>(
             new FuncPredicate(() =>
             {
                 return AIBrainStateDriver.HasLostTarget() && AIBrainStateDriver.PatrolPoints != null && AIBrainStateDriver.PatrolPoints.Length > 0;
             }));


            AIBrainStateDriver.StateMachine.AddTransition<AIAttackState, AIIdleState>(
            new FuncPredicate(() =>
             {
                 return AIBrainStateDriver.HasLostTarget() && (AIBrainStateDriver.PatrolPoints == null || AIBrainStateDriver.PatrolPoints.Length == 0);
             }));

        }
    }
}
