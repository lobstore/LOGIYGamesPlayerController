using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class RewardView : MonoBehaviour
    {
        [field: SerializeField] public Image Sprite { get; set; }
        [field: SerializeField] public TextMeshProUGUI Value { get; set; }
    }
}