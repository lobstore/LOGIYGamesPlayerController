using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class ToMousePointRotation : IRotationStrategy
    {
        Character Character { get; set; }
        public ToMousePointRotation(Character character)
        {
            Character = character;
        }
        public Quaternion GetRotation()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = (hitPoint - Character.transform.position);
                direction.y = 0f; // чтобы не заваливался вверх/вниз

                if (direction != Vector3.zero)
                {
                    return Quaternion.LookRotation(direction);
                }
                else
                {
                    return Quaternion.identity;
                }
            }
            else
            {
                return Quaternion.identity;
            }
        }
    }

}

