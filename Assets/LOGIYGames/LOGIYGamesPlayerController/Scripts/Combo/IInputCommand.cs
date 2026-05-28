using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
namespace LOGIYGames.CharacterCore
{

    public interface IInputCommand
    {
        AttackInputType InputType { get; }

        float Time { get; }
    }
   
}
