using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderClimbMovement : IMovementStrategy
    {
        CharacterModule Character;
        LadderMovementController Ladder;
        public LadderClimbMovement(CharacterModule character, LadderMovementController ladder)
        {
            Character = character;
            Ladder = ladder;
        }

        public Vector3 GetMovementDirection()
        {
            return Ladder.Ladder.GetPosition(Ladder.t)-Character.transform.position;
        }
    }
}

