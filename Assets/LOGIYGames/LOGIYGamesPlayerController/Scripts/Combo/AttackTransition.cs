using LOGIYGames.Shared.Enums;
using System;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class AttackTransition
    {
        public AttackInputType Input;

        public AttackNodeSO NextAttack;
    }
}
