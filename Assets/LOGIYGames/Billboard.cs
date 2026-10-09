using UnityEngine;

namespace LOGIYGames
{
    public class Billboard : MonoBehaviour
    {
        private Transform mainCameraTransform;

        void Start()
        {
            // Находим трансформ главной камеры при старте
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
        }

        void LateUpdate()
        {
            if (mainCameraTransform != null)
            {
                // Поворачиваем объект в ту же сторону, куда смотрит камера
                transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                                 mainCameraTransform.rotation * Vector3.up);
            }
        }
    }
}
