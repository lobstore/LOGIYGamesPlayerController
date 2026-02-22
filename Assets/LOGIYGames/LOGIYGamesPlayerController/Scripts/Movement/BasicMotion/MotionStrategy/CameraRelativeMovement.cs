using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraRelativeMovement : IMovementStrategy
    {
        Character Character { get; set; }
        public CameraRelativeMovement(Character character)
        {
            Character = character;
        }
        public Vector3 GetMovementDirection()
        {
            Vector3 movement = new Vector3(Character.InputProvider.MovementInput.x, 0, Character.InputProvider.MovementInput.y);

            Transform cam = Camera.main.transform;

            // Берём направления камеры
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            // Обнуляем вертикальную составляющую, чтобы не было движения вверх/вниз
            camForward.y = 0f;
            camRight.y = 0f;

            // Нормализуем, чтобы избежать ускорения при наклоне камеры
            camForward.Normalize();
            camRight.Normalize();

            // Рассчитываем направление движения относительно камеры
            Vector3 move = (camRight * movement.x) + (camForward * movement.z);

            return move.normalized;
        }
    }

}

