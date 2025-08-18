using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }
        List<CinemachineCameraController> cinemachineCameraControllers = new();
        public CinemachineCameraController CurentCameraController { get; private set; }
        [SerializeField] CinemachineCameraController FPSCameraController;
        [SerializeField] CinemachineCameraController TPSCameraController;
        [SerializeField] CinemachineCameraController TDSCameraController;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            if (Instance==null)
            {
                Instance = this;
            }
            
            InitControllersViews();
        }
        public void SetTargetTo(Transform Follow, Transform LookAt)
        {
            foreach (var cam in cinemachineCameraControllers)
            {
                cam.CameraFollowTarget = Follow;
                cam.CameraLookAtTarget = LookAt;
            }
        }

        private void InitControllersViews()
        {
            cinemachineCameraControllers.Add(FPSCameraController);
            cinemachineCameraControllers.Add(TPSCameraController);
            cinemachineCameraControllers.Add(TDSCameraController);
        }


        public void SetTPView()
        {
            CurentCameraController = TPSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
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
        public void SetFPView()
        {
            CurentCameraController = FPSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
        }
        public void SetTDView()
        {
            CurentCameraController = TDSCameraController;
            SetPriorVirtualCamera(CurentCameraController);
        }

    }
}
