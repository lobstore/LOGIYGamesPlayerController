using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class FlyMovement : IMovementStrategy
    {
        Character Character;

        public FlyMovement(Character character)
        {
            Character = character;
        }

        public Vector3 GetMovementDirection()
        {
            Vector3 movement = new Vector3(Character.Input.MovementInput.x, 0, Character.Input.MovementInput.y);

            Transform cam = Camera.main.transform;

            // Берём направления камеры
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            // Нормализуем, чтобы избежать случайных ускорений
            camForward.Normalize();
            camRight.Normalize();

            // Формируем итоговое направление в пространстве камеры
            Vector3 move = (camRight * movement.x) + (camForward * movement.z);
            return move.normalized;
        }
    }
}

