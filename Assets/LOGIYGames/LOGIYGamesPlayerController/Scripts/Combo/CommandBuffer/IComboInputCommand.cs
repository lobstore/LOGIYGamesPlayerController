using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
namespace LOGIYGames.CharacterCore
{

    public interface IComboInputCommand
    {
        AttackInputType InputType { get; }

        float Time { get; }
    }
   
}
