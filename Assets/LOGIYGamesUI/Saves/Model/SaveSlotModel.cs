using UnityEngine.Events;
namespace LOGIYGames
{
    public class SaveSlotModel
    {
        private GameData data = null;

        public UnityEvent OnDataChanged { get; set; } = new();
        public bool IsEmpty { get => data == null; }
        public float Progress { get; set; }
        public string ProfileId { get; set; }
        public int DeathCount { get; set; }


        public GameData Data { get { return data; } set { data = value; OnDataChanged.Invoke(); } }
    }
}