using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
public class ComboMovementState : MovementStateBase
{
    ComboController combo;
    public ComboMovementState(Actor character, MovementStateData data) : base(character, data)
    {
        combo = character.GetComponent<ComboController>();
    }

    public override void Enter()
    {
        base.Enter();
        _character.MovementStrategy = new NoneMovement();
        _character.RotationStrategy = new NoneRotation(_character);

        combo.BeginCombo();
        _character.ResetInput();
    }
    public override void Exit()
    {
        base.Exit();
        combo.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        combo.Tick();
    }
    public bool CanDodge()
    {
        return combo.IsDodgeWindowActive();
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