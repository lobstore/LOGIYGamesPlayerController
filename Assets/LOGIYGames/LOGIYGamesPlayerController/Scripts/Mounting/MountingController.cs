using LOGIYGames;
using LOGIYGames.CharacterCore;
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
        Target.GetComponent<PlayerInput>().enabled = true;
        CameraManager.Instance.CameraInput.Disable();
        CameraManager.Instance.DisableAllCameras();
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<MovementWrapperBase>().UseGravity= false;
        transform.SetParent(Current.transform);
        transform.localPosition = Vector3.zero;
    }

    public void Dismount()
    {
        Current.GetComponent<PlayerInput>().enabled = false;
        gameObject.GetComponent<MovementWrapperBase>().UseGravity = true;
        CameraManager.Instance.CameraInput.Enable();
        CameraManager.Instance.EnableAllCameras();
        transform.position = Current.transform.position + -Current.transform.right * 2;
        transform.SetParent(null);
        gameObject.GetComponent<Collider>().enabled = true;
        Current = null;
        Target = null;
        ExitRequested = false;
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