using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderClimbRotation : IRotationStrategy
    {
        LadderClimbController Ladder;

        public LadderClimbRotation(LadderClimbController ladder)
        {
            Ladder = ladder;
        }

        public Quaternion GetRotation()
        {
            Vector3 up = Ladder.GetCurrentDirection(); // вверх по лестнице
            Vector3 right = Vector3.Cross(Vector3.forward, up).normalized;

            Vector3 forward = Vector3.Cross(up, right).normalized;

            return Quaternion.LookRotation(forward, up);

        }
    }
}

