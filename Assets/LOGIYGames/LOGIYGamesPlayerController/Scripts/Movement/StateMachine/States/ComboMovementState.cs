using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
public class ComboMovementState : CharacterMovementState
{
    ComboController combo;
    public ComboMovementState(Character character, MovementStateData data) : base(character, data)
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

        combo.ResetCombo();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        combo.ReadInput();
    }
    public bool CanExit()
    {
        return true;
    }


}