using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
public class ComboMovementState : MovementStateBase
{
    ComboController combo;
    public ComboMovementState(Actor character, MovementStateData data) : base(character, data)
    {
        //combo = character.ComboController;
    }

    public override void Enter()
    {
        base.Enter();
        _character.MovementStrategy = new NoneMovement();
        _character.RotationStrategy = new NoneRotation(_character);

        // _character.ComboController.BeginCombo();
        _character.ResetInput();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        combo.Tick();
    }
    public override bool CanExit()
    {
        return combo.CanExit();
    }

    public override bool CanEnter()
    {
        return combo.CanEnter();
    }
}