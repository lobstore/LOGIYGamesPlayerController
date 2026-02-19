using UnityEngine;

namespace LOGIYGames
{
    public class ModelSlopeAdjuster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform modelTransform; // Модель персонажа

        [Header("Slope Settings")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float slopeCheckDistance = 1f;
        [SerializeField] private LayerMask groundLayerMask = 1;

        [Header("Adjustment Settings")]
        [SerializeField] private float adjustmentSpeed = 5f;
        [SerializeField] private float maxModelDrop = 1f;

        private Vector3 originalModelLocalPosition;
        private float currentDropAmount = 0f;
        private RaycastHit slopeHit;

        void Start()
        {
            if (modelTransform == null)
            {
                // Если модель не назначена, используем сам объект
                modelTransform = transform;
            }

            originalModelLocalPosition = modelTransform.localPosition;

            // Автоматически находим CharacterController если не назначен
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (characterController == null)
            {
                Debug.LogError("CharacterController not found! Please assign one.");
            }
        }

        void Update()
        {
            AdjustModelToSlope();
        }

        private void AdjustModelToSlope()
        {
            if (characterController == null || modelTransform == null) return;

            bool isOnSlope = CheckForSlope();
            float targetDropAmount = 0f;

            if (isOnSlope)
            {
                // Рассчитываем величину опускания в зависимости от угла наклона
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float normalizedAngle = Mathf.Clamp01(slopeAngle / maxSlopeAngle);
                targetDropAmount = normalizedAngle * maxModelDrop;
            }

            // Плавно интерполируем текущее значение опускания
            currentDropAmount = Mathf.Lerp(currentDropAmount, targetDropAmount, adjustmentSpeed * Time.deltaTime);

            // Применяем опускание к модели
            Vector3 newPosition = originalModelLocalPosition;
            newPosition.y -= currentDropAmount;
            modelTransform.localPosition = newPosition;
        }

        private bool CheckForSlope()
        {
            // Бросаем луч вниз от центра CharacterController
            Vector3 rayOrigin = transform.position + characterController.center;

            if (Physics.Raycast(rayOrigin, Vector3.down, out slopeHit,
                               characterController.height / 2 + slopeCheckDistance, groundLayerMask))
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return slopeAngle > 1f && slopeAngle <= maxSlopeAngle;
            }

            return false;
        }

        // Визуализация в редакторе
        private void OnDrawGizmosSelected()
        {
            if (characterController != null)
            {
                Vector3 rayOrigin = transform.position + characterController.center;
                Vector3 rayEnd = rayOrigin + Vector3.down * (characterController.height / 2 + slopeCheckDistance);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(rayOrigin, rayEnd);

                if (slopeHit.collider != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(slopeHit.point, 0.1f);
                    Gizmos.DrawRay(slopeHit.point, slopeHit.normal);
                }
            }
        }
    }
}
