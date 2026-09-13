using UnityEngine;

namespace LOGIYGames
{

    public class Mountable : MonoBehaviour
    {
        [Header("Mount")]
        [SerializeField] private Transform mountPoint;
        [SerializeField] private Transform dismountPoint;

        [Header("Steering Wheel IK")]
        [SerializeField] private Transform leftHandGrip;
        [SerializeField] private Transform rightHandGrip;

        private Transform mountedObject;
        private HandsIK handsIK;

        public void Mount(Transform objectToMount)
        {
            if (objectToMount == null || mountPoint == null)
                return;

            mountedObject = objectToMount;

            objectToMount.SetParent(mountPoint);
            objectToMount.SetPositionAndRotation(
                mountPoint.position,
                mountPoint.rotation
            );

            objectToMount.localPosition = Vector3.zero;

            SetupSteeringWheelIK(objectToMount);
        }

        public void Dismount()
        {
            if (mountedObject == null || dismountPoint == null)
                return;

            DisableSteeringWheelIK();

            mountedObject.SetParent(null);
            mountedObject.SetPositionAndRotation(
                dismountPoint.position,
                dismountPoint.rotation
            );

            mountedObject = null;
            handsIK = null;
        }

        private void SetupSteeringWheelIK(Transform character)
        {
            if (character == null)
                return;

            handsIK = character.GetComponent<HandsIK>();

            if (handsIK == null)
                return;

            if (leftHandGrip == null || rightHandGrip == null)
                return;

            handsIK.LeftHandPoint = leftHandGrip.position;
            handsIK.RightHandPoint = rightHandGrip.position;

            handsIK.LeftHandNormal = leftHandGrip.forward;
            handsIK.RightHandNormal = rightHandGrip.forward;

            handsIK.EnableIK();
        }

        private void DisableSteeringWheelIK()
        {
            if (handsIK == null)
                return;

            handsIK.DisableIK();
        }

        private void Update()
        {
            if (mountedObject == null)
                return;

            UpdateSteeringWheelIK();
        }

        private void UpdateSteeringWheelIK()
        {
            if (handsIK == null)
                return;

            if (leftHandGrip == null || rightHandGrip == null)
                return;

            handsIK.LeftHandPoint = leftHandGrip.position;
            handsIK.RightHandPoint = rightHandGrip.position;

            handsIK.LeftHandNormal = leftHandGrip.forward;
            handsIK.RightHandNormal = rightHandGrip.forward;
        }
    }
}
