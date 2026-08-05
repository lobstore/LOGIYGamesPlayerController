using LOGIYGames.Shared.Enums;
using System;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class AttackInputCommand
    : IComboInputCommand
    {
        public AttackInputType InputType
        {
            get;
            private set;
        }

        public float Time
        {
            get;
            private set;
        }

        public AttackInputCommand(
            AttackInputType inputType)
        {
            InputType = inputType;

            Time = UnityEngine.Time.time;
        }
    }
   
}
