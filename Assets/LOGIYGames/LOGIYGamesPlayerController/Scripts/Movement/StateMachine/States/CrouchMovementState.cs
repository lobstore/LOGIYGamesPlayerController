using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class CrouchMovementState : MovementStateBase
    {

        LocomotionController controller;

        public CrouchMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<LocomotionController>();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                controller.Move();

            }
            if (!Data.IsAnimationDrivenRotation)
            {
                _character.Rotate();

            }
        }
        public override void Enter()
        {
            base.Enter();
            _animator.SetBool("IsCrouching", true);
            
        }

        public override void Exit()
        {
            base.Exit();
            _animator.SetBool("IsCrouching", false);
        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && _character.Input.CrouchPressing;

        }
        public override bool CanExit()
        {
            return base.CanExit()
                && !_character.Input.CrouchPressing;
        }
    }

}
