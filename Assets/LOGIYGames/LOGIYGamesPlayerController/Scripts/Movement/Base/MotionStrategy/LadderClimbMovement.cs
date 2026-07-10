using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderClimbMovement : IMovementStrategy
    {
        Character Character;
        LadderClimbController Ladder;
        public LadderClimbMovement(Character character, LadderClimbController ladder)
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

