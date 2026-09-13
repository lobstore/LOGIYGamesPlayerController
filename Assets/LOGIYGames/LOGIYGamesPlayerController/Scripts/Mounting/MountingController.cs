using LOGIYGames;
using LOGIYGames.Animation;
using LOGIYGames.CharacterCore;
using RealStep;
using UnityEngine;
using UnityEngine.InputSystem;

public class MountingController : MonoBehaviour
{
    Actor actor;
    private GameObject Current;
    private GameObject Target;
    public bool ExitRequested {  get; private set; }
    public bool IsMounted => Current != null;
    private void Awake()
    {
        actor = GetComponent<Actor>();
    }
    private void Update()
    {
        if (!ExitRequested && IsMounted)
        {
            ExitRequested = actor.Input.InteractPressed;
        }
    }
    public bool CanMount()
    {
        return Target != null;
    }

    public bool CanDismount()
    {
        return true;
    }

    public void Mount()
    {
        Current = Target;
        Current.GetComponent<PlayerInput>().enabled = true;
        CameraManager.Instance.CameraInput.Disable();
        CameraManager.Instance.DisableAllCameras();
        PlayerManager.Instance.PlayerInput.DisableMovement();
        GetComponent<FootIK>().enabled = false;
        GetComponent<MovementWrapperBase>().IsNoClip = true;
        GetComponent<CharacterAnimationController>().PlayAnimation("Driving");
        GetComponent<MovementWrapperBase>().DisableMovement();
        Current.GetComponent<Mountable>().Mount(transform);
    }

    public void Dismount()
    {
        Current.GetComponent<Mountable>().Dismount();
        Current.GetComponent<PlayerInput>().enabled = false;
        GetComponent<FootIK>().enabled = true;
        GetComponent<MovementWrapperBase>().IsNoClip = false;
        GetComponent<CharacterAnimationController>().PlayAnimation("Exiting Car");
        GetComponent<MovementWrapperBase>().EnableMovement();
        Current = null;
        Target = null;
        ExitRequested = false;
        PlayerManager.Instance.PlayerInput.EnableMovement();
        CameraManager.Instance.CameraInput.Enable();
        CameraManager.Instance.EnableAllCameras();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mount"))
        {
            Target = other.gameObject;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mount"))
        {
            Target = null;
        }
    }
}