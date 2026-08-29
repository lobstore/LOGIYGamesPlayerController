using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class CharacterActionState : IState
    {
        protected Actor _character;
        protected CharacterActionState(Actor character)
        {
            _character = character;
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
}
