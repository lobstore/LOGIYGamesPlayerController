using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine.Events;
using LOGIYGames.Shared.Extensions;
namespace LOGIYGames
{
    public class MantlingMovementState : MovementStateBase
    {
        #region Fields
        private MantlingController MantlingController;

        #endregion

        public MantlingMovementState(Actor ctx, MantlingMovmentStateData stateData) : base(ctx, stateData)
        {
            MantlingController = ctx.gameObject.GetOrAddComponent<MantlingController>();
        }


        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new NoneRotation(_character);
            _character.MovementStrategy = new NoneMovement();
            _controller.UseGravity = false;
            _controller.IsNoClip = true;
            MantlingController.BeginMantling();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            MantlingController.Tick();
        }
        public override void Exit()
        {
            base.Exit();
            MantlingController.Cancel();

            _controller.UseGravity = true;
            _controller.IsNoClip = false;
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && MantlingController.CanEnter();
        }
        public override bool CanExit()
        {
            return MantlingController.CanExit();
        }
    }
}