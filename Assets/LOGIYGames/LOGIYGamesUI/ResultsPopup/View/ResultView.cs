using TMPro;
using UnityEngine;
namespace LOGIYGames
{
    public class ResultView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Name { get; set; }
        [field: SerializeField] public TextMeshProUGUI Value { get; set; }
    }
}