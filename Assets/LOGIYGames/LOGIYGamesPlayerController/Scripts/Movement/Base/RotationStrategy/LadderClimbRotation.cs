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
            if (Ladder.Ladder != null)
            {
                Vector3 up = Ladder.Ladder.GetDirection(Ladder.t); // вверх по лестнице
                Vector3 right = Vector3.Cross(Vector3.forward, up).normalized;

                Vector3 forward = Vector3.Cross(up, right).normalized;

                return Quaternion.LookRotation(forward, up);
            }

            return Ladder.transform.rotation;

        }
    }
}

