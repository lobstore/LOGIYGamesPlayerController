using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class ActionBaseState : IState
    {
        protected Character Character;
        protected ActionBaseState(Character character)
        {
            Character = character;
        }
        public virtual void Enter()
        {
           
        }

        public virtual void Exit()
        {
            
        }

        public virtual void LateUpdate()
        {
         
        }

        public virtual void LogicUpdate()
        {
          
        }

        public virtual void PhysicsUpdate()
        {
           
        }
    }
    public class ThrowItemActionState : ActionBaseState
    {
        public ThrowItemActionState(Character character) : base(character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Character.EventBus.Publish(new ItemThrowedEvent());
        }
    }
}
