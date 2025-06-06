using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class ProductView : MonoBehaviour
    {

        [field: SerializeField] public Button buyButton { get; private set; }

        [field: SerializeField] public Image Image { get; set; }

        [field: SerializeField] public TextMeshProUGUI Descrtiption { get; set; }
        [field: SerializeField] public TextMeshProUGUI Cost { get; set; }

        [field: SerializeField] public GameObject restrictGo;
    }
}