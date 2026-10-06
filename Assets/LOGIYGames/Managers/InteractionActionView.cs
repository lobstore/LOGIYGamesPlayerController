using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class InteractionActionView : MonoBehaviour
    {
        [SerializeField] RectTransform root;
        [SerializeField] GameObject prefab;
        IReadOnlyList<InteractionAction> actions;
        InteractionContext context;
        private Camera targetCamera;
        private void Awake()
        {
            targetCamera = Camera.main;
        }
        public void Show(IReadOnlyList<InteractionAction> actions, InteractionContext context)
        {
            this.actions = actions;
            this.context = context;
            Clean();
            Populate();
            Enable();
        }

        public void Enable()
        {
            root.GetComponent<Canvas>().enabled = true;
        }

        public void Disable()
        {
            root.GetComponent<Canvas>().enabled = false;
        }

        private void Populate()
        {
            foreach (var action in actions)
            {
                var obj = Instantiate(prefab);
                obj.transform.SetParent(root);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    action.Execute(context);
                    Hide();
                    PlayerManager.Instance.PlayerInput.Enable();
                    CameraManager.Instance.CameraInput.Enable();
                    CameraManager.Instance.ResetCameraView();
                });
                obj.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = action.ViewData.Text;
            }
        }
        private void LateUpdate()
        {
            if (targetCamera == null)
                return;

            transform.forward =
                targetCamera.transform.position - transform.position;
        }
        private void Clean()
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Destroy(root.GetChild(i).gameObject);
            }
        }

        public void Hide()
        {
            Disable();
        }
    }
}
