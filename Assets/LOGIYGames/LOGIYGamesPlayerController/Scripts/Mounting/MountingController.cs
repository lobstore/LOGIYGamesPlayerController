using LOGIYGames;
using LOGIYGames.Animation;
using LOGIYGames.CharacterCore;
using RealStep;
using UnityEngine;

public class MountingController : MonoBehaviour
{
    Actor actor;
    MovementWrapperBase controller;
    private GameObject Current;
    private GameObject Target;
    public bool ExitRequested { get; private set; }
    public bool IsMounted => Current != null;
    private void Awake()
    {
        actor = GetComponent<Actor>();
        controller = GetComponent<MovementWrapperBase>();
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
        //Current.GetComponent<PlayerInput>().enabled = true;
        //CameraManager.Instance.CameraInput.Disable();
        //CameraManager.Instance.DisableAllCameras();
        //PlayerManager.Instance.InputReader.DisableMovement();
        GetComponent<FootIK>().enabled = false;
        controller.IsNoClip = true;
        controller.UseGravity = false;
        GetComponent<Animator>().SetBool("IsDriving", true);
        GetComponent<CharacterAnimationController>().PlayAnimation("Driving");
        controller.DisableMovement();
        Current.GetComponent<Mountable>().Mount(transform);

    }

    public void Dismount()
    {
        Current.GetComponent<Mountable>().Dismount();
        //Current.GetComponent<PlayerInput>().enabled = false;
        GetComponent<FootIK>().enabled = true;
        controller.IsNoClip = false;
        controller.UseGravity = true;
        GetComponent<Animator>().SetBool("IsDriving", false);

        //GetComponent<CharacterAnimationController>().PlayAnimation("Exiting Car");
        controller.EnableMovement();
        Current = null;
        Target = null;
        ExitRequested = false;
        // PlayerManager.Instance.InputReader.EnableMovement();
        // CameraManager.Instance.CameraInput.Enable();
        //CameraManager.Instance.EnableAllCameras();
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