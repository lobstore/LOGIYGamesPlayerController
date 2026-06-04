using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboBufferDebugView
    : MonoBehaviour
    {
        [SerializeField]
        private Vector2 position =
            new Vector2(20, 20);

        public InputCommandBuffer Buffer
        {
            get;
            set;
        }

        private GUIStyle style;

        private void Awake()
        {
            style = new GUIStyle();

            style.fontSize = 24;

            style.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            if (Buffer == null)
                return;

            GUI.Label(
                new Rect(
                    position.x,
                    position.y,
                    1000,
                    100),
                $"INPUT BUFFER: {Buffer.GetDebugBuffer()}",
                style);
        }
    }
}
