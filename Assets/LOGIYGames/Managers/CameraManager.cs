using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public enum CameraPerspectiveType
    {
        FirstPerson,
        ThirdPersonFreeLook,
        ThirdPersonLookForward,
        Top_Down
    }
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] List<CinemachineCameraController> cinemachineCameraControllers = new();
        public CinemachineCameraController CurrentCameraController { get; private set; }
        [SerializeField] CinemachineCameraController FirstPersonCameraController;
        [SerializeField] CinemachineCameraController ThirdPersonCameraController;
        [SerializeField] CinemachineCameraController TopDownCameraController;

        CinemachineCameraController instance_FirstPersonCameraController;
        CinemachineCameraController instance_ThirdPersonCameraController;
        CinemachineCameraController instance_TopDownCameraController;


        [field: SerializeField] public PlayerCameraInputReader CameraInput {  get; private set; }
        [SerializeField] public CameraPerspectiveType CameraPerspectiveType;
        protected override void Initialize()
        {

            instance_FirstPersonCameraController = Instantiate(FirstPersonCameraController, null);
            instance_ThirdPersonCameraController = Instantiate(ThirdPersonCameraController, null);
            instance_TopDownCameraController = Instantiate(TopDownCameraController, null);
            cinemachineCameraControllers.Add(instance_FirstPersonCameraController);
            cinemachineCameraControllers.Add(instance_ThirdPersonCameraController);
            cinemachineCameraControllers.Add(instance_TopDownCameraController);
        }
        private void Start()
        {
            CameraInput.Enable();
            Set3rdFreeLookView();
        }
        int index = 0;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                index++;
                index = index % cinemachineCameraControllers.Count;
                if (index == 0)
                {
                    Set3rdFreeLookView();
                }
                else if (index == 1)
                {
                    Set1stView();
                }
                else if (index == 2)
                {
                    SetTopDownView();
                }else if (index == 3)
                {
                    Set3rdLookForwardView();
                }
            }

        }
        public void SetTargetTo(Transform Follow, Transform LookAt)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.CameraFollowTarget = Follow;
                cam.CameraLookAtTarget = LookAt;
            }
        }



        void SetPriorVirtualCamera(CinemachineCameraController cameraController)
        {
            foreach (var controller in cinemachineCameraControllers)
            {
                if (controller != cameraController)
                {
                    controller.Priority = 0;
                    continue;
                }
                else
                {
                    controller.Priority = 10;
                }

            }

        }
        public void Set3rdFreeLookView()
        {
            CurrentCameraController = instance_ThirdPersonCameraController;
            CameraPerspectiveType = CameraPerspectiveType.ThirdPersonFreeLook;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void Set1stView()
        {
            CurrentCameraController = instance_FirstPersonCameraController;
            CameraPerspectiveType = CameraPerspectiveType.FirstPerson;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void SetTopDownView()
        {
            CurrentCameraController = instance_TopDownCameraController;
            CameraPerspectiveType = CameraPerspectiveType.Top_Down;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void Set3rdLookForwardView()
        {
            CurrentCameraController = instance_ThirdPersonCameraController;
            CameraPerspectiveType = CameraPerspectiveType.ThirdPersonLookForward;
            SetPriorVirtualCamera(CurrentCameraController);
        }

    }
}
