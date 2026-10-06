using LOGIYGames.CharacterCore;
using Unity.Cinemachine;

namespace LOGIYGames
{
    public class LockOnModule : CinemachineExtension
    {
        Actor actor;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Awake()
        {
            base.Awake();
            PlayerManager.Instance.OnCharacterChanged.AddListener((chr) =>
            {
                actor = chr;
            });
        }

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime)
        {
            if (!actor.TargetingController.HasTarget)
            {
                CameraManager.Instance.ResetCameraView();
                return;
            }

            if (vcam is CinemachineCamera camera)
            {
                var target = camera.Target;
                target.CustomLookAtTarget = true;
                target.LookAtTarget = actor.TargetingController.CurrentTarget;
                camera.Target = target;
            }
        }
    }
}
