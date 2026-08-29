using UnityEngine;

namespace LOGIYGames
{
    public class Door : InteractableObject
    {
        public bool openOnStart;
        public bool IsLocked { get; private set; }
        public bool IsOpened { get; private set; }
        override protected void Start()
        {
            base.Start();
            if (openOnStart)
            {
                Open();
            }
        }
        public void Open()
        {
            if (IsLocked) return;

            IsOpened = true;
            Debug.Log("Door opened");
        }

        public void Close()
        {
            IsOpened = false;
            Debug.Log("Door closed");
        }

        public void Lock()
        {
            if (IsLocked) return;
            IsLocked = true;
            Debug.Log("Door locked");
        }
        public void UnLock()
        {
            if (!IsLocked) return;
            IsLocked = false;
            Debug.Log("Door unlocked");
        }

    }
}

