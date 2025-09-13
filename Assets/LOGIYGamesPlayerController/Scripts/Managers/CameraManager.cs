using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public enum CameraPerspectiveType
    {
        FirstPerson,
        ThirdPerson,
        Top_Down
    }
    public class CameraManager : Singleton<CameraManager>
    {
        List<CinemachineCameraController> cinemachineCameraControllers = new();
        public CinemachineCameraController CurentCameraController { get; private set; }
        [SerializeField] CinemachineCameraController FPSCameraController;
        [SerializeField] CinemachineCameraController TPSCameraController;
        [SerializeField] CinemachineCameraController TDSCameraController;
        [SerializeField] public CameraPerspectiveType CameraPerspectiveType = CameraPerspectiveType.ThirdPerson;
        protected override void Initialize()
        {
            cinemachineCameraControllers.Add(FPSCameraController);
            cinemachineCameraControllers.Add(TPSCameraController);
            cinemachineCameraControllers.Add(TDSCameraController);
           
        }
        private void Start()
        {
            SetTPView();
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
                    SetTPView();
                }
                else if (index == 1)
                {
                    SetFPView();
                }
                else if(index == 2)
                {
                    SetTDView();
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
        public void SetTPView()
        {
            CurentCameraController = TPSCameraController;
            CameraPerspectiveType = CameraPerspectiveType.ThirdPerson;
            SetPriorVirtualCamera(CurentCameraController);
        }
        public void SetFPView()
        {
            CurentCameraController = FPSCameraController;
            CameraPerspectiveType = CameraPerspectiveType.FirstPerson;
            SetPriorVirtualCamera(CurentCameraController);
        }
        public void SetTDView()
        {
            CurentCameraController = TDSCameraController;
            CameraPerspectiveType = CameraPerspectiveType.Top_Down;
            SetPriorVirtualCamera(CurentCameraController);
        }
        
    }
}
