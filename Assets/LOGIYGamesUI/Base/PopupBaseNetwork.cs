using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
    public abstract class PopupBaseNetwork : NetworkBehaviour
    {
        [SerializeField] protected bool HideOnStart;
        public bool IsShowing { get; private set; }
        protected virtual void Start()
        {
            if (HideOnStart)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
        public virtual void Show() { GetComponent<Canvas>().enabled = true; IsShowing = true; }
        public virtual void Hide() { GetComponent<Canvas>().enabled = false; IsShowing = false; }
    }
}