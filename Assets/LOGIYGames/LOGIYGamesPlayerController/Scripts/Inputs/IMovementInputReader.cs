using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IMovementInputReader
    {
        Vector2 MovementInput { get;}

        bool FocusPressed { get;}
        bool JumpPressed { get;}
        bool EvadePressed { get;}
        bool SprintPressing { get;}
        bool CrouchPressed { get;}

        bool AttackPressed { get;}

    }
}
