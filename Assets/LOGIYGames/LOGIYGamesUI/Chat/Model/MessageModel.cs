using Unity.Netcode;
namespace LOGIYGames
{
    public class MessageModel : INetworkSerializable
    {
        public string Text;
        public string PlayerName;
        public string PlayerId;
        public bool IsOwner;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Text);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref IsOwner);
        }
    }
}