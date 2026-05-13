using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [SerializeField] InputActionAsset inputActions;
        public PlayerCameraInputReader CameraInput { get; private set; }
        [field:SerializeField] private CameraPerspectiveType cameraPerspectiveType;

        public CameraPerspectiveType CameraPerspectiveType
        {
            get { return cameraPerspectiveType; }
            set
            {
                cameraPerspectiveType = value;
                UpdateCameraView();
            }
        }
        protected override void Initialize()
        {
            CameraInput = new(inputActions);
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
            UpdateCameraView();

        }

        private void UpdateCameraView()
        {
            switch (CameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    Set1stView();
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    Set3rdFreeLookView();
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    Set3rdLookForwardView();
                    break;
                case CameraPerspectiveType.Top_Down:
                    SetTopDownView();
                    break;
                default:
                    break;
            }
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
                    CameraPerspectiveType = CameraPerspectiveType.ThirdPersonFreeLook;
                }
                else if (index == 1)
                {
                    CameraPerspectiveType = CameraPerspectiveType.FirstPerson;
                }
                else if (index == 2)
                {
                    CameraPerspectiveType = CameraPerspectiveType.Top_Down;
                }
                else if (index == 3)
                {
                    CameraPerspectiveType = CameraPerspectiveType.ThirdPersonLookForward;
                }
            }
            UpdateCameraView();

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
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void Set1stView()
        {
            CurrentCameraController = instance_FirstPersonCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void SetTopDownView()
        {
            CurrentCameraController = instance_TopDownCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }
        public void Set3rdLookForwardView()
        {
            CurrentCameraController = instance_ThirdPersonCameraController;
            SetPriorVirtualCamera(CurrentCameraController);
        }

    }
}
