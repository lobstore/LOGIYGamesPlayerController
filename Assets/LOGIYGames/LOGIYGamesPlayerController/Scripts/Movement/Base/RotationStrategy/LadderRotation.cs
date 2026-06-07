using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderRotation : IRotationStrategy
    {
        Transform Ladder;

        public LadderRotation(Transform ladder)
        {
            Ladder = ladder;
        }

        public Quaternion GetRotation()
        {
            return Ladder.rotation;
        }
    }

}

