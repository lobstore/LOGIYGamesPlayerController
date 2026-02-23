using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToTargetRotation : IRotationStrategy
    {
        Character Character;
        public ToTargetRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            if (Character.Target == null)
            {
                float currentAngleY = Character.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, currentAngleY, 0f);
                return targetRotation;
            }
            else
            {
                Vector3 targetDirection = Character.Target.position - Character.transform.position;
                targetDirection.y = 0f;
                return Quaternion.LookRotation(targetDirection);
            }
        }
    }
}
