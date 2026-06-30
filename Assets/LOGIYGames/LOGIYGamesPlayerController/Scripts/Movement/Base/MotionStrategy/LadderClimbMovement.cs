using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderClimbMovement : IMovementStrategy
    {
        Character Character;
        LadderMovementController Ladder;
        public LadderClimbMovement(Character character, LadderMovementController ladder)
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

