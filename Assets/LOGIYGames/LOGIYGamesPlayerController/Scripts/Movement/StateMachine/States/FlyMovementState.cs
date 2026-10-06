using LOGIYGames.CharacterCore;
using UnityEngine;
namespace LOGIYGames.Movement
{
    public class FlyMovementState : MovementStateBase
    {
        public FlyMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        Vector3 dir;
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new Input360LookMovement(_character);
            _character.GetComponent<MovementWrapperBase>().UseGravity = false;
            _animator.SetBool("IsFlying", true);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                _controller.ChangeVelocity(_character.RuntimeMovement.TargetDirection * _character.RuntimeMovement.CurrentSpeed * 3.5f);

            }
            if (!Data.IsAnimationDrivenRotation)
            {
                _character.Rotate();

            }
        }
        public override bool CanExit()
        {
            return base.CanExit() && !_character.Input.SubAttackPressed;
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.SubAttackPressed;
        }
        public override void Exit()
        {
            base.Exit();
            _animator.SetBool("IsFlying", false);
            _character.GetComponent<MovementWrapperBase>().UseGravity = true;
        }
    }
}
