using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class LobbyItemView : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI LobbyName;
        [SerializeField] public TextMeshProUGUI LobbyMembersCount;
        [SerializeField] public TextMeshProUGUI LobbyPrivacyStatus;
        [SerializeField] public Button Button;
    }
}