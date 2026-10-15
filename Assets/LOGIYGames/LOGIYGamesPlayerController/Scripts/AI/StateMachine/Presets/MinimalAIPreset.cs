using LOGIYGames.AI;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(
        fileName = "MinimalAIPreset",
        menuName = "AIStateMachine/AIStatesPreset/MinimalAI")]
    public class MinimalAIPreset : AIStatesBuilder
    {
        public override void Build(AIBrain brain)
        {
            StateMachine machine = brain.StateMachine;

            machine.AddState(new AIIdleState(brain));
            machine.AddState(new AIPatrolState(brain));
            machine.AddState(new AIChaseState(brain));
            machine.AddState(new AICombatState(brain));

            ConfigureTransitions(brain);

            machine.SetState<AIIdleState>();
        }

        private void ConfigureTransitions(AIBrain brain)
        {
            StateMachine machine = brain.StateMachine;

            // Начать бой, когда цель подошла достаточно близко.
            machine.AddAnyTransition<AICombatState>(
                new FuncPredicate(() =>
                    brain.HasTarget &&
                    brain.DistanceToTarget <= brain.CombatEnterRange));

            // Покинуть боевой режим только после выхода за больший радиус.
            // В состоянии Chase используется радиус входа в бой.
            machine.AddAnyTransition<AIChaseState>(
                new FuncPredicate(() =>
                    brain.HasTarget &&
                    brain.DistanceToTarget >
                    (machine.IsInState<AICombatState>()
                        ? brain.CombatExitRange
                        : brain.CombatEnterRange)));

            machine.AddAnyTransition<AIPatrolState>(
                new FuncPredicate(() =>
                    !brain.HasTarget &&
                    brain.HasPatrolPoints));

            machine.AddAnyTransition<AIIdleState>(
                new FuncPredicate(() =>
                    !brain.HasTarget &&
                    !brain.HasPatrolPoints));
        }
    }
}