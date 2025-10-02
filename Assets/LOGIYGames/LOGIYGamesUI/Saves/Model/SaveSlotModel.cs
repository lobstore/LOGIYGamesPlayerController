using SaveIsEasy;
using UnityEngine.Events;
namespace LOGIYGames
{
    public class SaveSlotModel
    {
        private SceneFile data = null;

        public UnityEvent OnDataChanged { get; set; } = new();
        public bool IsEmpty { get => data == null; }
        public float Progress { get; set; }
        public int SlotId { get; set; }
        public int DeathCount { get; set; }
        public SceneFile Data { get { return data; } set { data = value; OnDataChanged.Invoke(); } }
    }
}