using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface ICharacterInputReader
    {
        Vector2 MovementInput { get;}

        bool FocusPressed { get;}
        bool JumpPressed { get;}
        bool EvadePressed { get;}
        bool SprintPressing { get;}
        bool CrouchPressed { get;}
        bool InteractPressed { get;}
        bool AttackPressed { get;}

    }
}
