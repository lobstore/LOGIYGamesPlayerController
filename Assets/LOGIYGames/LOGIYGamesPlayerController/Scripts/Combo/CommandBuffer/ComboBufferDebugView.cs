using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboBufferDebugView : MonoBehaviour
    {
        [SerializeField]
        private Vector2 position =
            new Vector2(20, 20);

        private ComboController ComboController;
        [SerializeField] private Character Character;

        private GUIStyle style;

        private void Awake()
        {

            style = new GUIStyle();

            style.fontSize = 24;

            style.normal.textColor = Color.white;
        }
        private void Start()
        {
            ComboController = Character.ComboController;
        }
        private void OnGUI()
        {
            if (ComboController.CommandBuffer == null)
                return;

            GUI.Label(
                new Rect(
                    position.x,
                    position.y,
                    1000,
                    100),
                $"INPUT BUFFER: {ComboController.CommandBuffer.GetDebugBuffer()}",
                style);
        }
    }
}
