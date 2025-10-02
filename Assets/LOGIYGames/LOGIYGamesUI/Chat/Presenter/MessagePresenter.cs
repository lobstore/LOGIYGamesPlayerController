namespace LOGIYGames
{
    public class MessagePresenter
    {
        public MessageModel MessageModel { get; private set; }
        public MessageView MessageView { get; private set; }

        public MessagePresenter(MessageModel messageModel, MessageView messageView)
        {
            MessageModel = messageModel;
            MessageView = messageView;

            MessageView.MessageText.text = MessageModel.Text;
            MessageView.SenderNameText.text = MessageModel.PlayerName;
        }

    }
}