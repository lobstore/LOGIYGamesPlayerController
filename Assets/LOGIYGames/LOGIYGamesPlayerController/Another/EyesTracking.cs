using UnityEngine;

namespace LOGIYGames
{
    public class EyesTracking : MonoBehaviour
    {
        [SerializeField] Vrm10LookAtModule lookAt;
        [SerializeField] NearestInteractableFinder nearestInteractableFinder;
        private void Update()
        {
            Tracking();
        }

        private void Tracking()
        {
            lookAt.TargetTransform = nearestInteractableFinder.CurrentTarget?.transform;
        }
    }
}
