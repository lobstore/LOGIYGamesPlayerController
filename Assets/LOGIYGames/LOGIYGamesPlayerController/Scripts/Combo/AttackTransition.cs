using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class AttackTransition
    {
        public InputSequence Sequence;

        public AttackNodeSO NextAttack;
    }
    [Serializable]
    public class InputSequence
    {
        public List<AttackInputType> Inputs = new();
    }
}
