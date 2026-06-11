using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class InputPlanarRotation : IRotationStrategy
    {
        CharacterModule Character;

        public InputPlanarRotation(CharacterModule character)
        {
            Character = character;
        }

        public Quaternion GetRotation()
        {

            Vector2 input = Character.Input.MovementInput;

            // если есть ввод — поворачиваем по направлению камеры
            if (input.magnitude > 0)
            {

                // берем направления камеры по плоскости XZ
                Vector3 cameraForward = Character.Input.LookForward;
                Vector3 cameraRight = Character.Input.LookRight;

                cameraForward.y = 0f;
                cameraRight.y = 0f;

                cameraForward.Normalize();
                cameraRight.Normalize();

                // направление движения относительно камеры
                Vector3 moveDir =
                    cameraForward * input.y +
                    cameraRight * input.x;

                if (moveDir.magnitude > 0)
                {
                    return Quaternion.LookRotation(moveDir);
                }
            }

            // если нет ввода — оставляем текущий поворот по Y
            Vector3 currentForward = Character.transform.forward;
            currentForward.y = 0f;

            if (currentForward.sqrMagnitude < 0.0001f)
                currentForward = Vector3.forward;

            return Quaternion.LookRotation(currentForward);
        }
    }
}
