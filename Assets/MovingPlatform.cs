using UnityEngine;

namespace LOGIYGames
{
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Movement (Stable Path)")]

        [SerializeField]
        private Vector3 moveDirection = Vector3.forward;

        [SerializeField]
        private float moveDistance = 3f;

        [SerializeField]
        private float moveSpeed = 2f;

        [SerializeField]
        private bool localMovement = false;

        [Header("Rotation (Independent)")]

        [SerializeField]
        private Vector3 rotationAxis = Vector3.up;

        [SerializeField]
        private float rotationSpeed = 30f;

        [Header("Debug")]

        [SerializeField]
        private bool drawGizmos = true;

        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            // -------- ROTATION (does NOT affect movement) --------
            transform.Rotate(
                rotationAxis,
                rotationSpeed * Time.deltaTime,
                Space.Self
            );

            // -------- MOVEMENT (fixed direction path) --------
            float pingPong =
                Mathf.PingPong(Time.time * moveSpeed, moveDistance);

            Vector3 direction =
                localMovement
                    ? moveDirection.normalized
                    : moveDirection.normalized;

            Vector3 offset =
                direction * pingPong;

            transform.position =
                _startPosition + offset;
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = Color.cyan;

            Vector3 start =
                Application.isPlaying
                    ? _startPosition
                    : transform.position;

            Vector3 dir =
                moveDirection.normalized;

            Vector3 end =
                start + dir * moveDistance;

            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(start, 0.1f);
            Gizmos.DrawSphere(end, 0.1f);
        }
    }
}