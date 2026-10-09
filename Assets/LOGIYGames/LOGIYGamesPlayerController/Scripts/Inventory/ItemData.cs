using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(menuName = "Item/Data")]
    public class ItemData : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public bool IsStackable = false;
        public int MaxStackSize = 100;

        public void OnEnable() => Name ??= name;
    }
}
