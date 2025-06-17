using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class ChatPopup : PopupBaseNetwork
    {
        [SerializeField] GameObject fromPlayerMessagePrefab;
        [SerializeField] GameObject fromAnotherMessagePrefab;
        [SerializeField] Button sendMessageButton;
        [SerializeField] TMP_InputField InputField;
        [SerializeField] Transform viewTransform;

        [SerializeField] ScrollRect scrollRect;
        [SerializeField] RectTransform content;

        [Header("Settings")]

        [SerializeField] Image NewMessageBlob;

        [SerializeField] float messagesLimit;

        private Queue<MessagePresenter> _messageQueue = new Queue<MessagePresenter>();
        public UnityEvent<MessagePresenter> OnGotNewMessage { get; private set; } = new();

        private int _unreadCount;

        public string playerName;
        public string playerID;
        protected override void Start()
        {
            playerName = "Player" + Random.Range(0, 10000);
            playerID = Random.Range(0, 10000).ToString();

            base.Start();
        }
        private void OnEnable()
        {
            sendMessageButton.onClick.AddListener(SendMessage);
            InputField.onSelect.AddListener(InputFieldInFocus);
            InputField.onDeselect.AddListener(InputFieldOutFocus);
            OnGotNewMessage.AddListener(GotNewMessage);
        }
        private void OnDisable()
        {
            sendMessageButton.onClick.RemoveListener(SendMessage);
            InputField.onSelect.RemoveListener(InputFieldInFocus);
            InputField.onDeselect.RemoveListener(InputFieldOutFocus);
            OnGotNewMessage.RemoveListener(GotNewMessage);
        }
        private void GotNewMessage(MessagePresenter newMessage)
        {
            NewMessageBlob.gameObject.SetActive(true);
        }

        private void InputFieldInFocus(string str)
        {
            PlayerInputsManager.Instance.gameObject.SetActive(false);
            CameraInputManager.Instance.gameObject.SetActive(false);
        }
        private void InputFieldOutFocus(string str)
        {
            PlayerInputsManager.Instance.gameObject.SetActive(true);
            CameraInputManager.Instance.gameObject.SetActive(true);
        }
        private void SendMessage()
        {
            CreateMessage();
        }
        private void MessageCountControl()
        {
            // Если очередь переполнена — удаляем самое старое сообщение
            if (_messageQueue.Count > messagesLimit)
            {
                var oldMessage = _messageQueue.Dequeue().MessageView;
                Destroy(oldMessage.gameObject);
            }
        }
        private void CreateMessage()
        {
            if (!string.IsNullOrWhiteSpace(InputField.text))
            {
                var model = new MessageModel { PlayerName = playerName, Text = InputField.text, PlayerId = playerID };
                InputField.text = "";
                CreateMessageServerRpc(model);
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void CreateMessageServerRpc(MessageModel model)
        {
            CreateMessageClientRpc(model);
        }

        [ClientRpc]
        private void CreateMessageClientRpc(MessageModel model)
        {
            if (model == null)
            {
                return;
            }
            GameObject go = null;
            if (model.PlayerId == playerID)
            {
                go = Instantiate(fromPlayerMessagePrefab, viewTransform);
            }
            else
            {
                go = Instantiate(fromAnotherMessagePrefab, viewTransform);
            }
            go.transform.SetParent(viewTransform, false);
            Canvas.ForceUpdateCanvases();
            var presenter = new MessagePresenter(model, go.GetComponent<MessageView>());
            _messageQueue.Enqueue(presenter);
            if (!IsOwner && !IsShowing)
            {
                OnGotNewMessage?.Invoke(presenter);
            }

            MessageCountControl();
            ScrollToBottom();

        }

        private void ScrollToBottom()
        {
            if (scrollRect == null || content == null) return;

            scrollRect.verticalNormalizedPosition = 0f;

        }
        public override void Hide()
        {
            base.Hide();
        }

        public override void Show()
        {
            base.Show();
            NewMessageBlob.gameObject.SetActive(false);
        }
    }
}