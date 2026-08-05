using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineRollEffect : CinemachineExtension
{
    [Header("Roll Settings")]
    public float rollSpeed = 8f;

    Character player; // объект где находится IsRolling

    RollMovementState roll;
    LandingMovementState land;
    private float currentRotation;
    protected override void Awake()
    {
        base.Awake();
        PlayerManager.Instance.OnCharacterChanged.AddListener((chr) =>
        {
            player = chr;
            roll = player.GetMovementState<RollMovementState>();
            land = player.GetMovementState<LandingMovementState>();
        });
    }
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Aim)
            return;

        if (player == null)
            return;

        if (roll.IsActiveState)
        {
            // крутим камеру во время переката
            currentRotation += rollSpeed * deltaTime;
        }
        else
        {
            // возвращаем накопленный угол после переката
            currentRotation = 0;
        }
        Vector3 localForward = state.RawOrientation * Vector3.right;
        Quaternion rollAngle = Quaternion.AngleAxis(
            currentRotation,
            localForward
        );

        state.RawOrientation = rollAngle * state.RawOrientation;
    }
}