using Alchemy.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField, Required] TextMeshProUGUI stackSize;
        [SerializeField, Required] Image icon;
        [SerializeField, Required] Sprite missingIconSprite;

        Item item;

        public Image Icon => icon;

        public void SetItem(Item value)
        {
            item = value;
            if (item == null) return;
            if (item.Data == null)
            {
                icon.sprite = missingIconSprite;
                stackSize.enabled = false;
                stackSize.text = string.Empty;
                return;
            }
            icon.sprite = item.Icon ?? missingIconSprite;
            stackSize.enabled = item.Stackable && item.StackSize > 1;
            stackSize.text = item.StackSize.ToString();
        }
    }
}
