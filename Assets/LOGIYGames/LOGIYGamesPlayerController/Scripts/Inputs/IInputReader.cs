using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IInputReader
    {
        Vector2 MovementInput { get;}

        bool FocusPressed { get;}
        bool JumpPressed { get;}
        bool EvadePressed { get;}
        bool SprintPressed { get;}
        bool CrouchPressed { get;}

    }
}
