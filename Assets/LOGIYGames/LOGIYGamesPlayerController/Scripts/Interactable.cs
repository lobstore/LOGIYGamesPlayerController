using UnityEngine;
namespace LOGIYGames
{
    public class Interactable : MonoBehaviour
    {
        [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public bool IsTouchable { get; private set; }
        [field: SerializeField] public bool IsMoving { get; private set; }
        [field: SerializeField] public Transform Origin { get; private set; }
        public bool IsActive { get; private set; }
        private Vector3 _previousPosition;
        private void LateUpdate()
        {
            IsMoving = (transform.position != _previousPosition);
            _previousPosition = transform.position;

        }
    }
}