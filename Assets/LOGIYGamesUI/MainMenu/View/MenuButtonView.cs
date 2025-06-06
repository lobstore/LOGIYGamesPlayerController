using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class MenuButtonView : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }

        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
    }
}