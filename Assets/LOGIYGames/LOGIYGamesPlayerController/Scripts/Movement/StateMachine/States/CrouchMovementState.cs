using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class CrouchMovementState : MovementStateBase
    {

        LocomotionController controller;
        protected float StandingHeight;
        protected float CrouchHeight;

        public CrouchMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<LocomotionController>();
            StandingHeight = _controller.Height;
            CrouchHeight = StandingHeight * 0.5f;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _controller.Height = Mathf.MoveTowards(_controller.Height, CrouchHeight, Time.fixedDeltaTime);
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
            _controller.Height = StandingHeight;
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
