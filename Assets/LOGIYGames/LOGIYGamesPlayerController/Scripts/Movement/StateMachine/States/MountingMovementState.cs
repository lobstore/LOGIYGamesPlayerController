using LOGIYGames.CharacterCore;
using UnityEngine;
namespace LOGIYGames.Movement
{
    public class MountingMovementState : MovementStateBase
    {
        MountingController controller;
        public MountingMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<MountingController>();
        }
        public override bool CanEnter()
        {
            Debug.Log(_character.Input.InteractPressed);
            return controller.CanMount() && _character.Input.InteractPressed;
        }
        public override bool CanExit()
        {
            return controller.IsMounted && controller.CanDismount() && _character.Input.InteractPressed;
        }
        public override void Enter()
        {
            base.Enter();
            controller.Mount();
        }
        public override void Exit()
        {
            base.Exit();
            controller.Dismount();
        }
    }
}
